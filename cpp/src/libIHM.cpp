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
#ifndef M_PI
#define M_PI 3.14159265358979323846
#endif


// Initialisateur par défaut
ClibIHM::ClibIHM() {

	this->nbDataImg = 0;
	this->dataFromImg.clear();
	this->dataObject.clear();
	this->imgPt = NULL;
}

// Initialisateur par valeurs
//modif
ClibIHM::ClibIHM(int nbChamps, byte* data, int stride, int nbLig, int nbCol)
{
	std::cout << "Réception des paramètres dans ClibIHM :" << std::endl;
	std::cout << " - Nombre de canaux : " << nbChamps << std::endl;
	std::cout << " - Stride : " << stride << std::endl;
	std::cout << " - Nombre de lignes : " << nbLig << std::endl;
	std::cout << " - Nombre de colonnes : " << nbCol << std::endl;

	if (data == nullptr) {
		std::cerr << "ERREUR : Pointeur de données NULL !" << std::endl;
		throw std::runtime_error("Pointeur de données invalide.");
	}

	if (nbLig <= 0 || nbCol <= 0 || stride <= 0) {
		std::cerr << "ERREUR : Dimensions invalides !" << std::endl;
		throw std::runtime_error("Dimensions invalides.");
	}

	nbDataImg = nbChamps;
	dataFromImg.resize(nbChamps);
	this->data = data;
	this->NbLig = nbLig;
	this->NbCol = nbCol;
	this->stride = stride;

	bool estVide = true;
	for (int i = 0; i < nbLig * nbCol; i++) {
		if (data[i] != 0) {
			estVide = false;
			break;
		}
	}

	if (estVide) {
		std::cerr << "ERREUR : L'image reçue est complètement noire !" << std::endl;
		throw std::runtime_error("L'image chargée est vide !");
	}

	std::cout << "Allocation des images..." << std::endl;
	imgPt = new CImageCouleur(nbLig, nbCol);
	imgNdgPt = new CImageNdg(nbLig, nbCol);

	if (!imgPt || !imgNdgPt) {
		std::cerr << "ERREUR : Allocation mémoire échouée !" << std::endl;
		throw std::runtime_error("Erreur mémoire");
	}

	std::cout << "Chargement des pixels..." << std::endl;

	byte* pixPtr = this->data;
	for (int y = 0; y < nbLig; y++) {
		for (int x = 0; x < nbCol; x++) {
			imgPt->operator()(y, x)[0] = pixPtr[3 * x + 2];
			imgPt->operator()(y, x)[1] = pixPtr[3 * x + 1];
			imgPt->operator()(y, x)[2] = pixPtr[3 * x];

			imgNdgPt->operator()(y, x) = (int)(0.299 * pixPtr[3 * x] + 0.587 * pixPtr[3 * x + 1] + 0.114 * pixPtr[3 * x + 2]);
		}
		pixPtr += stride;
	}

	std::cout << "Image correctement chargée en mémoire !" << std::endl;
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

// Projet Traitement de l'image
void ClibIHM::runProcess(ClibIHM* pImgGt)
{
	int seuilBas = 0;
	int seuilHaut = 255;

	CImageNdg inv_whiteTopHat, whiteTopHat;

	//filtre median
	this->filter("median", 3, "V8");

	// Creation et demarrage des threads pour calculer whiteTopHat et inv_whiteTopHat
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
// projet vision
// modif
// //problème
 // Reconnaissance de couleur
void ClibIHM::runProcessCap() {
    if (imgPt == nullptr) {
        throw std::runtime_error("ERREUR : Image couleur non chargée.");
    }

    std::cout << "Début du traitement d'image pour détection des objets." << std::endl;

    // sEUILLAGE POUR DÉTECTER LES OBJETS**
    CImageNdg binaryMask(NbLig, NbCol, 0);
    for (int y = 0; y < NbLig; y++) {
        for (int x = 0; x < NbCol; x++) {
            auto& pixel = imgPt->operator()(y, x);

            // Seuillage (Segmentation des objets)
            if (pixel[0] > 50 || pixel[1] > 50 || pixel[2] > 50) 
			{
                binaryMask(y, x) = 255; // Objet
            } else {
                binaryMask(y, x) = 0; // Fond
            }
        }
    }

    //  ÉTIQUETAGE DES OBJETS**
    CImageClasse formes(binaryMask, "V8");
    int nbRegions = formes.lireNbRegions();
    std::cout << "Nombre d'objets détectés : " << nbRegions << std::endl;

    if (nbRegions == 0) {
        std::cerr << "ERREUR : Aucun objet détecté !" << std::endl;
        return;
    }

    // CRÉATION D'UNE IMAGE DE SORTIE**
    CImageCouleur resultImage(NbLig, NbCol);

    // ANALYSE DES FORMES ET COULEURS**
    for (int regionIndex = 1; regionIndex <= nbRegions; regionIndex++) {
        float cgX = 0, cgY = 0;
        int surface = 0;

        // Calcul du centre de gravité et de la surface de l’objet
        for (int y = 0; y < NbLig; y++) {
            for (int x = 0; x < NbCol; x++) {
                if (formes(y, x) == regionIndex) {
                    cgX += x;
                    cgY += y;
                    surface++;
                }
            }
        }

        if (surface == 0) continue;

        int posX = static_cast<int>(cgX / surface);
        int posY = static_cast<int>(cgY / surface);

        //  DÉTECTION DE LA FORME
        float perimeter = 0.0f;
        for (int y = 1; y < NbLig - 1; y++) {
            for (int x = 1; x < NbCol - 1; x++) {
                if (formes(y, x) == regionIndex) {
                    if (formes(y - 1, x) == 0 || formes(y + 1, x) == 0 ||
                        formes(y, x - 1) == 0 || formes(y, x + 1) == 0) {
                        perimeter += 1.0f;
                    }
                }
            }
        }

        float circularity = 4 * M_PI * surface / (perimeter * perimeter);
        std::string detectedShape = (circularity > 0.8f) ? "Cercle" : (circularity > 0.5f) ? "Carré" : "Triangle";

        // DÉTECTION DE LA COULEUR RGB
        int totalR = 0, totalG = 0, totalB = 0;
        for (int y = 0; y < NbLig; y++) {
            for (int x = 0; x < NbCol; x++) {
                if (formes(y, x) == regionIndex) {
                    auto& pixel = imgPt->operator()(y, x);
                    totalR += pixel[2];  // Rouge
                    totalG += pixel[1];  // Vert
                    totalB += pixel[0];  // Bleu
                }
            }
        }

        int avgR = totalR / surface;
        int avgG = totalG / surface;
        int avgB = totalB / surface;

        std::string detectedColor;
        if (avgR > avgG && avgR > avgB) detectedColor = "Rouge";
        else if (avgG > avgR && avgG > avgB) detectedColor = "Vert";
        else if (avgB > avgR && avgB > avgG) detectedColor = "Bleu";
        else detectedColor = "Inconnue";

        //  AFFICHAGE DES RÉSULTATS**
        std::cout << "Objet " << regionIndex << " : " << detectedShape << " " << detectedColor
                  << " à (" << posX << "," << posY << ")" << std::endl;

        // COLORATION DES OBJETS SUR L'IMAGE DE SORTIE
        for (int y = 0; y < NbLig; y++) {
            for (int x = 0; x < NbCol; x++) {
                if (formes(y, x) == regionIndex) {
                    resultImage(y, x)[0] = avgB; // Bleu
                    resultImage(y, x)[1] = avgG; // Vert
                    resultImage(y, x)[2] = avgR; // Rouge
                }
            }
        }
    }

    // AFFICHAGE ET SAUVEGARDE**
    this->writeImage(this, resultImage);
    std::cout << "Analyse terminée. Résultats affichés." << std::endl;
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

		this->ecrireChamp(0, floor(score * 10000) / 100);
	});

	std::thread th2([&] {
		// Score de Vinet
		CImageClasse imgClasse(img, "V8");

		double score = imgClasse.vinet(img, GT);

		this->ecrireChamp(1, floor(score * 10000) / 100);
	
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
	this->dataObject.clear();

}
// ajout peut être à enlever
// Méthode conversion de l'image en niveau de gris
CImageCouleur ClibIHM::convertirEnCouleur(const CImageNdg& imgNdg) {
	CImageCouleur imgCouleur(imgNdg.lireHauteur(), imgNdg.lireLargeur());

	for (int y = 0; y < imgNdg.lireHauteur(); y++) {
		for (int x = 0; x < imgNdg.lireLargeur(); x++) {
			int valeur = imgNdg(y, x);
			imgCouleur(y, x)[0] = valeur; // Bleu
			imgCouleur(y, x)[1] = valeur; // Vert
			imgCouleur(y, x)[2] = valeur; // Rouge
		}
	}

	return imgCouleur;
}
