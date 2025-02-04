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
	this->dataObject.clear();
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

// Traitement de l'image pour la détection des bouchons
void ClibIHM::runProcessCap(int threshold, int sizeMin) {
	try {
		if (this->imgNdgPt == nullptr) {
			std::cerr << "Erreur: imgNdgPt est nul." << std::endl;
			return;
		}

		// Filtrage de l'image
		CImageNdg binaryImg;
		try {
			binaryImg = this->imgNdgPt->filtrage("moyennage", 3, 3, "disk");
		}
		catch (const std::exception& ex) {
			std::cerr << "Erreur lors du filtrage: " << ex.what() << std::endl;
			return;
		}

		int seuilHaut = 255;
		// Seuillage manuel
		CImageNdg man;
		try {
			man = binaryImg.seuillage("manuel", threshold, seuilHaut);
		}
		catch (const std::exception& ex) {
			std::cerr << "Erreur lors du seuillage: " << ex.what() << std::endl;
			return;
		}

		// Extraction des bouchons
		CImageNdg trueRes;
		try {
			std::vector<Bouchon> bouchons = extractBouchons(man, trueRes, sizeMin);
			// Ecriture des objets
			for (const auto& bouchon : bouchons) {
				std::ostringstream oss;
				oss << bouchon.couleur << ", "
					<< bouchon.forme << ", "
					<< bouchon.centroidX_mm << ", "
					<< bouchon.centroidY_mm;
				char* s = _strdup(oss.str().c_str());
				// Stockage des objets
				this->ecrireObject(bouchon.label, s);
				// Libération de la mémoire
				free(s);
			}
		}
		catch (const std::exception &ex) {
			std::cerr << "Erreur lors de l'extraction des objets: " << ex.what() << std::endl;
		}
		
		try {
			this->writeBinaryImage(trueRes);
		}
		catch (const std::exception& ex) {
			std::cerr << "Erreur lors de l'écriture de l'image binaire: " << ex.what() << std::endl;
		}

		try {
			this->persitData(this->imgNdgPt, COULEUR::RVB);
		}
		catch (const std::exception& ex) {
			std::cerr << "Erreur lors de la persistance des données: " << ex.what() << std::endl;
		}
	}
	catch (const std::exception& ex) {
		std::cerr << "Exception dans runProcessCap: " << ex.what() << std::endl;
	}
	catch (...) {
		std::cerr << "Exception inconnue dans runProcessCap." << std::endl;
	}
}

// Extraction des bouchons
std::vector<Bouchon> ClibIHM::extractBouchons(const CImageNdg& img, CImageNdg& trueRes, int sizeMin)
{
	// Définition de la structure pour les directions
	std::vector<Bouchon> bouchons;
	try {
		// CImageClasse pour obtenir les signatures
		CImageClasse imgClasse(img, "V8");
		CImageClasse filtre = imgClasse.filtrage("taille", sizeMin, 70000, false);
		trueRes = filtre.toNdg();
		// Récupération des signatures
		std::vector<SIGNATURE_Forme> labels = filtre.signatures();
		if (labels.empty()) {
			std::cerr << "Aucune signature détectée." << std::endl;
			return bouchons;
		}
		// Nombre de bouchons
		int nbBouchons = static_cast<int>(labels.size()) - 1;
		this->ecrireChamp(0, nbBouchons);
		// Modification de la taille du vecteur
		this->dataObject.resize(nbBouchons);

		// Récupération des dimensions de l'image
		int largeur = trueRes.lireLargeur();
		int hauteur = trueRes.lireHauteur();

		if (largeur <= 0 || hauteur <= 0) {
			std::cerr << "Dimensions invalides : " << largeur << "x" << hauteur << std::endl;
			return bouchons;
		}

		auto safeGetPixel = [&](int x, int y) -> int {
			if (x < 0 || x >= largeur || y < 0 || y >= hauteur)
				return 0;
			try {
				return trueRes(y, x);
			}
			catch (...) {
				return 0;
			}
			};

		// Conversion des pixels en mm
		float physicalRadiusMm = 175.0f;
		float imageRadiusPx = static_cast<float>(min(largeur, hauteur)) / 2.0f;
		float globalFacteurConversion = physicalRadiusMm / imageRadiusPx;

		// Centre de l'image
		float centerX = static_cast<float>(largeur) / 2.0f;
		float centerY = static_cast<float>(hauteur) / 2.0f;

		for (int i = 1; i < static_cast<int>(labels.size()); ++i)
		{
			try {
				Bouchon bouchon;
				bouchon.label = i - 1;

				// Centre de gravité
				float objX_px = static_cast<float>(labels[i].centreGravite_j);
				float objY_px = static_cast<float>(labels[i].centreGravite_i);

				// Calcul des rayons dans 4 directions
				std::vector<Direction> directions = { {0, -1}, {0, 1}, {-1, 0}, {1, 0} };
				std::vector<float> rayons;
				rayons.reserve(directions.size());

				// Coordonnées du centre
				int x0 = static_cast<int>(objX_px);
				int y0 = static_cast<int>(objY_px);

				float r_top = 0.0f, r_bottom = 0.0f, r_left = 0.0f, r_right = 0.0f;
				// Calcul des rayons dans les 4 directions
				for (size_t k = 0; k < directions.size(); ++k)
				{
					int x = x0;
					int y = y0;
					float rayon = 0.0f;
					while (x >= 0 && x < largeur && y >= 0 && y < hauteur)
					{
						int pixelValue = safeGetPixel(x, y);
						if (pixelValue == 0) {
							break;
						}
						x += directions[k].dx;
						y += directions[k].dy;
						rayon += 1.0f;
					}
					rayons.push_back(rayon);
					if (k == 0) r_top = rayon;
					else if (k == 1) r_bottom = rayon;
					else if (k == 2) r_left = rayon;
					else if (k == 3) r_right = rayon;
				}
				// Calcul du rayon moyen
				float rayonP_px = 0.0f;
				if (!rayons.empty()) {
					float sum = 0.0f;
					for (float r : rayons)
						sum += r;
					rayonP_px = sum / static_cast<float>(rayons.size());
				}
				else {
					rayonP_px = 1.0f; // Pour éviter la division par 0
				}
				// Conversion des coordonnées en mm
				float facteurConversion = globalFacteurConversion;
				float realX_mm = (objX_px - centerX) * facteurConversion;
				float realY_mm = (objY_px - centerY) * facteurConversion;
				float rayonP_mm = rayonP_px * facteurConversion;
				// Stockage des coordonnées
				bouchon.centroidX_mm = realX_mm;
				bouchon.centroidY_mm = realY_mm;
				bouchon.rayon_mm = rayonP_mm;

				// Stocker les rayons individuels convertis en mm
				bouchon.r_top = r_top * facteurConversion;
				bouchon.r_bottom = r_bottom * facteurConversion;
				bouchon.r_left = r_left * facteurConversion;
				bouchon.r_right = r_right * facteurConversion;

				// Détermination de la forme
				bouchon.forme = determineShape(bouchon);
				if (bouchon.forme.empty()) {
					std::cerr << "Forme inconnue pour le bouchon " << bouchon.label
						<< ", image non stockée." << std::endl;
					continue;
				}

				// Détermination de la couleur
				bouchon.couleur = determineColor(bouchon);

				// Stockage du bouchon
				bouchons.push_back(bouchon);
			}
			catch (const std::exception& ex) {
				std::cerr << "Erreur lors du traitement du bouchon à l'indice " << i
					<< " : " << ex.what() << std::endl;
				continue;
			}
			catch (...) {
				std::cerr << "Erreur inconnue lors du traitement du bouchon à l'indice " << i << std::endl;
				continue;
			}
		}
	}
	catch (const std::exception& ex) {
		std::cerr << "Erreur dans extractBouchons: " << ex.what() << std::endl;
	}
	catch (...) {
		std::cerr << "Erreur inconnue dans extractBouchons." << std::endl;
	}
	return bouchons;
}

// Détermination de la forme du bouchon
std::string ClibIHM::determineShape(const Bouchon& bouchon) {
	// Récupération des rayons individuels (en mm)
	float r_top = bouchon.r_top;
	float r_bottom = bouchon.r_bottom;
	float r_left = bouchon.r_left;
	float r_right = bouchon.r_right;

	// Calcul de la moyenne des rayons
	float average = (r_top + r_bottom + r_left + r_right) / 4.0f;

	// Calcul du minimum et du maximum des rayons en utilisant des appels imbriqués
	float minRay = min(min(r_top, r_bottom), min(r_left, r_right));
	float maxRay = max(max(r_top, r_bottom), max(r_left, r_right));

	float ratio = (maxRay != 0.0f) ? (minRay / maxRay) : 0.0f;

	//Cercle : les 4 rayons doivent être très proches (ratio élevé)
	if (ratio > 0.92f) {
		return "cercle";
	}
	// Coeur : le rayon en haut est significativement réduit par rapport au rayon en bas,
	//    avec une symétrie horizontale (différence entre r_left et r_right faible)
	if ((r_top < 0.65f * r_bottom) && (std::fabs(r_left - r_right) < 0.1f * average)) {
		return "coeur";
	}
	// Triangle : différence significative entre le haut et le bas ou entre la gauche et la droite
	if ((std::fabs(r_top - r_bottom) > 0.25f * average) ||
		(std::fabs(r_left - r_right) > 0.25f * average)) {
		return "triangle";
	}
	// Carré : différences faibles entre les rayons opposés (mais non assez homogènes pour un cercle)
	if ((std::fabs(r_top - r_bottom) < 0.15f * average) &&
		(std::fabs(r_left - r_right) < 0.15f * average)) {
		return "carre";
	}

	return "";
}

// Détermination de la couleur du bouchon
std::string ClibIHM::determineColor(const Bouchon& bouchon) {

	return "inconnue";
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

