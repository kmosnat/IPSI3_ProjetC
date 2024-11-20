#include <iostream>
#include <fstream>
#include <sstream>
#include <string>
#include <windows.h>
#include <cmath>
#include <vector>
#include <ctime>
#include <stack>

#include "libIHM.h"

// Initialisateur par défaut
ClibIHM::ClibIHM() {

	this->nbDataImg = 0;
	this->dataFromImg.clear();
	this->imgPt = NULL;
}

// Initialisateur par valeurs
ClibIHM::ClibIHM(int nbChamps, byte* data, int stride, int nbLig, int nbCol)
{
	if (data == nullptr) {
		throw std::invalid_argument("Aucune Data");
	}

    // Initialisation des variables
	nbDataImg = nbChamps;
	dataFromImg.resize(nbChamps);
	this->data = data;
	this->NbLig = nbLig;
	this->NbCol = nbCol;
	this->stride = stride;

    // Initialisation des images
	imgPt = new CImageCouleur(nbLig, nbCol);
	imgNdgPt = new CImageNdg(nbLig, nbCol);

    // Vérification de l'allocation
	if (!imgPt) {
		throw std::runtime_error("Erreur allocation CImageCouleur");
	}

    // Récupération des valeurs des pixels
	byte* pixPtr = this->data;

	for (int y = 0; y < nbLig; y++)
	{
		for (int x = 0; x < nbCol; x++)
		{
			// R�cup�ration des valeurs RGB
			imgPt->operator()(y, x)[0] = pixPtr[3 * x + 2]; // Bleu
			imgPt->operator()(y, x)[1] = pixPtr[3 * x + 1]; // Vert
			imgPt->operator()(y, x)[2] = pixPtr[3 * x];     // Rouge

			// Conversion en niveau de gris
			imgNdgPt->operator()(y, x) = (int)(0.299 * pixPtr[3 * x] + 0.587 * pixPtr[3 * x + 1] + 0.114 * pixPtr[3 * x + 2]);
		}

		pixPtr += stride;
	}

}

// Copie d'une image de la classe CImageNdg à une image pointeur ClibIHM
void ClibIHM::copyImage(CImageNdg img)
{
	for (int y = 0; y < NbLig; y++)
	{
		for (int x = 0; x < NbCol; x++)
		{
			this->imgNdgPt->operator()(y, x) = img(y, x);
		}
	}
}

// Ecriture de l'image
void ClibIHM::writeImage(ClibIHM* img, CImageCouleur out)
{
	// Ecriture de l'image
	byte* pixPtr = img->data;
	for (int y = 0; y < NbLig; y++)
	{
		for (int x = 0; x < NbCol; x++)
		{
			pixPtr[3 * x + 2] = out(y, x)[0]; // Bleu
			pixPtr[3 * x + 1] = out(y, x)[1]; // Vert
			pixPtr[3 * x] = out(y, x)[2];	 // Rouge
		}
		pixPtr += stride;
	}
}

// Pour écrire une image binaire de la classe CImageNdg en image de la classe ClibIHM
void ClibIHM::writeBinaryImage(CImageNdg img)
{
	for (int y = 0; y < NbLig; y++)
	{
		for (int x = 0; x < NbCol; x++)
		{
			if (img(y, x) == 1)
			{
				this->imgNdgPt->operator()(y, x) = 255;
			}
			else
			{
				this->imgNdgPt->operator()(y, x) = 0;
			}
		}
	}
}

// Pour binariser une image de la classe ClibIHM en vrai binaire pour la classe CImageNdg
CImageNdg ClibIHM::toBinaire()
{
	CImageNdg imgNdg(NbLig, NbCol);
	for (int y = 0; y < NbLig; y++)
	{
		for (int x = 0; x < NbCol; x++)
		{
			if (this->imgNdgPt->operator()(y, x) == 255)
			{
				imgNdg(y, x) = 1;
			}
			else
			{
				imgNdg(y, x) = 0;
			}
		}
	}
	return imgNdg;
}

// Filtrage de l'image en fonction des paramètres appliqués
void ClibIHM::filter(std::string methode, int kernel, std::string str)
{
	if (methode == "moyen")
	{
		this->copyImage(this->imgNdgPt->filtrage("moyennage", kernel, kernel, str));
	}
	else if (methode == "median")
	{
		this->copyImage(this->imgNdgPt->filtrage("median", kernel, kernel, str));
	}

	this->persitData(this->imgNdgPt, COULEUR::RVB);
}

// Traitement de l'image
void ClibIHM::runProcess(ClibIHM* pImgGt)
{
	int seuilBas = 0;
	int seuilHaut = 255;

	CImageNdg inv_whiteTopHat, whiteTopHat;

	//filtre median
	this->filter("median", 3, "V8");

	// Cr�ation et d�marrage des threads pour calculer whiteTopHat et inv_whiteTopHat
	std::thread th1([&] {
		inv_whiteTopHat = this->imgNdgPt->transformation().whiteTopHat("disk", 17);
		});
	std::thread th2([&] {
		whiteTopHat = this->imgNdgPt->whiteTopHat("disk", 17);
		});

	// Attendez que th1 et th2 terminent
	th1.join();
	th2.join();

	CImageNdg inv_seuil, seuil;

	// Utilisation des r�sultats dans de nouveaux threads
	std::thread th3([&] {
		inv_seuil = inv_whiteTopHat.seuillage("otsu", seuilBas, seuilHaut).morphologie("erosion", "V8", 9).morphologie("dilatation", "V8", 9);
		});
	std::thread th4([&] {
		seuil = whiteTopHat.seuillage("otsu", seuilBas, seuilHaut).morphologie("erosion", "V8", 9).morphologie("dilatation", "V8", 9);
		});

	// Attendez que th3 et th4 terminent
	th3.join();
	th4.join();

	CImageNdg res;
	CImageNdg GT = pImgGt->toBinaire();


	if (fabs(inv_seuil.correlation(GT)) > fabs(seuil.correlation(GT)))
	{
		res = inv_seuil;
	}
	else
	{
		res = seuil;
	}


	CImageClasse imgClasse = CImageClasse(res, "V8");
	CImageClasse filtre = imgClasse.filtrage("taille", 30, 10000, false);
	CImageNdg trueRes = filtre.toNdg();

	this->writeBinaryImage(trueRes);

    // Calcul du score et comparaison
	this->score(pImgGt);
	this->compare(pImgGt);


	this->persitData(this->imgNdgPt, COULEUR::RVB);
}

// Compare l'image traitee et la ground truth pour afficher les ressemblances et differences
void ClibIHM::compare(ClibIHM* pImgGt)
{
	CImageCouleur out(NbLig, NbCol);

	for (int y = 0; y < NbLig; y++)
	{
		for (int x = 0; x < NbCol; x++)
		{
			if (this->imgNdgPt->operator()(y, x) == pImgGt->imgNdgPt->operator()(y, x))
			{
				out(y, x)[0] = 0;
				out(y, x)[1] = 255;
				out(y, x)[2] = 0;
			}
			else
			{
				out(y, x)[0] = 255;
				out(y, x)[1] = 0;
				out(y, x)[2] = 0;
			}
			if (this->imgNdgPt->operator()(y, x) == 0 && pImgGt->imgNdgPt->operator()(y, x) == 0)
			{
				out(y, x)[0] = 0;
				out(y, x)[1] = 0;
				out(y, x)[2] = 0;

			}
		}
	}

	writeImage(pImgGt, out);
}

// Calcul du score en fonction de la ground truth
void ClibIHM::score(ClibIHM* pImgGt)
{
	// Score IOU
	CImageNdg img = this->toBinaire();
	img.ecrireBinaire(true);
	CImageNdg GT = pImgGt->toBinaire();
	GT.ecrireBinaire(true);

    // Création et démarrage des threads pour calculer les scores
	std::thread th1([&] {

		double score = img.indicateurPerformance(GT, "iou");

		this->dataFromImg.at(0) = floor(score * 10000) / 100;
	});

	std::thread th2([&] {
		// Score de Vinet
		CImageClasse imgClasse(img, "V8");

		double score = imgClasse.vinet(img, GT);

		this->dataFromImg.at(1) = floor(score * 10000) / 100;
	
	});
	
	th1.join();
	th2.join();
}

// Ecrire les pixels de l'image envoye selon le canal choisit
void ClibIHM::persitData(CImageNdg* pImg, COULEUR color)
{
	CImageCouleur out(NbLig, NbCol);

	// Conversion de l'image en couleur
	for (int y = 0; y < NbLig; y++)
	{
		for (int x = 0; x < NbCol; x++)
		{
			if (color == COULEUR::RVB)
			{
				out(y, x)[0] = pImg->operator()(y, x);
				out(y, x)[1] = pImg->operator()(y, x);
				out(y, x)[2] = pImg->operator()(y, x);
			}
			else if (color == COULEUR::rouge)
			{
				out(y, x)[0] = pImg->operator()(y, x);
				out(y, x)[1] = 0;
				out(y, x)[2] = 0;
			}
			else if (color == COULEUR::vert)
			{
				out(y, x)[0] = 0;
				out(y, x)[1] = pImg->operator()(y, x);
				out(y, x)[2] = 0;
			}
			else if (color == COULEUR::bleu)
			{
				out(y, x)[0] = 0;
				out(y, x)[1] = 0;
				out(y, x)[2] = pImg->operator()(y, x);
			}
		}
	}

	writeImage(this, out);
}

// Destructeur
ClibIHM::~ClibIHM() {
	
	if (imgPt)
		(*this->imgPt).~CImageCouleur(); 
	this->dataFromImg.clear();
}

