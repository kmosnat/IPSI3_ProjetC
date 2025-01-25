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

void ClibIHM::connectedComponentLabeling(CImageNdg& binaryImg, std::vector<std::vector<int>>& labels) {
	int rows = binaryImg.lireHauteur();
	int cols = binaryImg.lireLargeur(); // Assurez-vous que CImageNdg a une méthode getCols()
	labels.assign(rows, std::vector<int>(cols, 0));

	std::map<int, Label> labelMap;
	int nextLabel = 1;

	// Première passe
	for (int y = 0; y < rows; y++) {
		for (int x = 0; x < cols; x++) {
			if (binaryImg(y, x) == 1) { // Supposons que les bouchons sont marqués par 1
				std::vector<int> neighbors;
				// Vérifier les voisins (8-connectivité)
				for (int dy = -1; dy <= 0; dy++) {
					for (int dx = -1; dx <= 1; dx++) {
						if (dy == 0 && dx == 1) continue; // Exclure les pixels à droite
						int ny = y + dy;
						int nx = x + dx;
						if (ny >= 0 && ny < rows && nx >= 0 && nx < cols) {
							if (binaryImg(ny, nx) == 1 && labels[ny][nx] > 0) {
								neighbors.push_back(labels[ny][nx]);
							}
						}
					}
				}

				if (neighbors.empty()) {
					labels[y][x] = nextLabel;
					labelMap[nextLabel] = Label{ nextLabel, 0 };
					nextLabel++;
				}
				else {
					int minLabel = *std::min_element(neighbors.begin(), neighbors.end());
					labels[y][x] = minLabel;
					for (int lbl : neighbors) {
						if (lbl != minLabel) {
							int root1 = findRoot(lbl, labelMap);
							int root2 = findRoot(minLabel, labelMap);
							if (root1 != root2) {
								// Union par rang
								if (labelMap[root1].rank < labelMap[root2].rank) {
									labelMap[root1].parent = root2;
								}
								else {
									labelMap[root2].parent = root1;
									if (labelMap[root1].rank == labelMap[root2].rank) {
										labelMap[root1].rank++;
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Deuxième passe
	for (int y = 0; y < rows; y++) {
		for (int x = 0; x < cols; x++) {
			if (labels[y][x] > 0) {
				labels[y][x] = findRoot(labels[y][x], labelMap);
			}
		}
	}
}

// Méthodes d'extraction
std::vector<Bouchon> ClibIHM::extractBouchons(const std::vector<std::vector<int>>& labels) {
	std::map<int, Bouchon> bouchonMap;

	int rows = labels.size();
	int cols = labels[0].size();

	for (int y = 0; y < rows; y++) {
		for (int x = 0; x < cols; x++) {
			if (labels[y][x] > 0) {
				int lbl = labels[y][x];
				if (bouchonMap.find(lbl) == bouchonMap.end()) {
					bouchonMap[lbl] = Bouchon{ lbl, {}, 0.0, 0.0, "", "" };
				}
				bouchonMap[lbl].pixels.emplace_back(y, x);
				bouchonMap[lbl].centroidX += x;
				bouchonMap[lbl].centroidY += y;
			}
		}
	}

	std::vector<Bouchon> bouchons;
	for (auto& pair : bouchonMap) {
		int lbl = pair.first;
		Bouchon& bouchon = pair.second;

		int numPixels = bouchon.pixels.size();
		if (numPixels > 0) { // Assurez-vous de ne pas diviser par zéro
			bouchon.centroidX /= numPixels;
			bouchon.centroidY /= numPixels;
		}
		bouchons.push_back(bouchon);
	}

	return bouchons;
}

void ClibIHM::runProcessCap() {
	// Étape 1: Prétraitement
	this->filter("median", 3, "V8");

	// Étape 2: Binarisation
	CImageNdg binaryImg = this->toBinaire();

	// Étape 3: Étiquetage des composantes connexes
	std::vector<std::vector<int>> labels;
	connectedComponentLabeling(binaryImg, labels);

	// Étape 4: Extraction des bouchons
	std::vector<Bouchon> bouchons = extractBouchons(labels);

	// Étape 5: Analyse des formes
	for (auto& bouchon : bouchons) {
		bouchon.forme = determineShapeAdvanced(bouchon);
	}

	// Étape 6: Analyse des couleurs
	analyzeColors(bouchons);

	// Étape 7: Stockage des informations
	this->dataObject.resize(bouchons.size());
	for (size_t i = 0; i < bouchons.size(); ++i) {
		std::ostringstream oss;
		oss << "Bouchon " << i + 1 << ": "
			<< "Forme=" << bouchons[i].forme << ", "
			<< "Couleur=" << bouchons[i].couleur << ", "
			<< "Position=(" << static_cast<int>(bouchons[i].centroidX) << ", "
			<< static_cast<int>(bouchons[i].centroidY) << ")";
		this->ecrireObject(i, _strdup(oss.str().c_str()));
	}

	// Étape 8: Persistance des données
	this->persitData(this->imgNdgPt, COULEUR::RVB);
}

// Implémentation des autres méthodes...

// Exemple pour isCircle
bool ClibIHM::isCircle(const Bouchon& bouchon) {
	double perimeter = 0.0;
	double area = bouchon.pixels.size();
	// Calculer le périmètre (approximatif)
	for (size_t i = 0; i < bouchon.pixels.size(); i++) {
		int y = bouchon.pixels[i].first;
		int x = bouchon.pixels[i].second;
		// Vérifier si le pixel a au moins un voisin noir (0)
		bool isEdge = false;
		for (int dy = -1; dy <= 1 && !isEdge; dy++) {
			for (int dx = -1; dx <= 1 && !isEdge; dx++) {
				if (dy == 0 && dx == 0) continue;
				int ny = y + dy;
				int nx = x + dx;
				if (ny >= 0 && ny < NbLig && nx >= 0 && nx < NbCol) {
					if (imgNdgPt->operator()(ny, nx) == 0) {
						isEdge = true;
					}
				}
				else {
					isEdge = true;
				}
			}
		}
		if (isEdge) perimeter++;
	}

	double circularity = 4 * acos(-1) * (area / (perimeter * perimeter));
	// La circularité d'un cercle parfait est 1
	return (circularity > 0.75); // Seuil à ajuster
}

// Implémentation simplifiée de isHeart
bool ClibIHM::isHeart(const Bouchon& bouchon) {
	// Cette méthode nécessite une heuristique spécifique. Voici une approche simplifiée:
	// Vérifier la présence de pixels concaves ou d'un certain nombre de points sur la ligne médiane
	// Retourne vrai si une forme de cœur est détectée

	// Placeholder: Implémentez une méthode plus robuste si nécessaire
	return false;
}

std::string ClibIHM::determineShape(const Bouchon& bouchon) {
	// Calculer l'encombrement
	if (bouchon.pixels.empty()) {
		return "Inconnu";
	}

	int minY = bouchon.pixels[0].first, maxY = bouchon.pixels[0].first;
	int minX = bouchon.pixels[0].second, maxX = bouchon.pixels[0].second;

	for (const auto& pixel : bouchon.pixels) {
		int y = pixel.first;
		int x = pixel.second;
		if (y < minY) minY = y;
		if (y > maxY) maxY = y;
		if (x < minX) minX = x;
		if (x > maxX) maxX = x;
	}

	int width = maxX - minX + 1;
	int height = maxY - minY + 1;
	double aspectRatio = static_cast<double>(width) / height;

	// Approximations basées sur l'aspect ratio et la circularité
	if (isCircle(bouchon)) {
		return "Cercle";
	}

	// Ratio pour différencier les carrés des rectangles
	if (aspectRatio > 0.9 && aspectRatio < 1.1) {
		return "Carré";
	}

	// Détection du triangle basé sur la hauteur
	if (height > width) {
		return "Triangle";
	}

	return "Inconnu";
}


std::string ClibIHM::determineShapeAdvanced(const Bouchon& bouchon) {
	if (isCircle(bouchon)) {
		return "Cercle";
	}
	std::string shape = determineShape(bouchon);
	if (shape == "Inconnu") {
		if (isHeart(bouchon)) {
			return "Cœur";
		}
	}
	return shape;
}

std::string ClibIHM::determineColor(const Bouchon& bouchon) {
	if (bouchon.pixels.empty()) {
		return "Inconnu";
	}

	long sumGray = 0;
	for (const auto& pixel : bouchon.pixels) {
		int y = pixel.first;
		int x = pixel.second;
		sumGray += this->imgNdgPt->operator()(y, x);
	}
	double avgGray = static_cast<double>(sumGray) / bouchon.pixels.size();

	// Définir des seuils pour différentes "couleurs" en niveaux de gris
	if (avgGray > 220) return "Jaune";
	if (avgGray > 180) return "Orange";
	if (avgGray > 140) return "Vert Foncé";
	if (avgGray > 100) return "Rose";
	if (avgGray > 60) return "Bleu";
	return "Noir";
}


void ClibIHM::analyzeColors(std::vector<Bouchon>& bouchons) {
	for (auto& bouchon : bouchons) {
		bouchon.couleur = determineColor(bouchon);
	}
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

