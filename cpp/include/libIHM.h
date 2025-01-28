#pragma once

#include "ImageClasse.h"
#include "ImageNdg.h"
#include "ImageCouleur.h"
#include "ImageDouble.h"

#include <windows.h>
#include <thread>
#include <vector>
#include <map>

// Enumération des couleurs
enum class COULEUR
{
	RVB,
	rouge,
	vert,
	bleu
};

struct Bouchon {
	int label;
	std::vector<std::pair<int, int>> pixels;
	double centroidX;
	double centroidY;
	std::string forme;
	std::string couleur;
};

// Déclaration de la structure Label
struct Label {
	int parent;
	int rank;
};


class ClibIHM {

	///////////////////////////////////////
private:
	///////////////////////////////////////

	// data n�cessaires � l'IHM donc fonction de l'application cibl�e
	int						nbDataImg; 
	std::vector<double>		dataFromImg;
	std::vector<std::string> dataObject;
	CImageCouleur* imgPt;        
	CImageNdg* imgNdgPt;     
	byte* data;       
	int NbLig;
	int NbCol;
	int stride;

	///////////////////////////////////////
public:
	///////////////////////////////////////

	// constructeurs
	_declspec(dllexport) ClibIHM(); // Initialisateur par défaut

    // Initialisateur par valeurs
	_declspec(dllexport) ClibIHM(int nbChamps, byte* data, int stride, int nbLig, int nbCol);

	_declspec(dllexport) ~ClibIHM();

	// get et set 

	_declspec(dllexport) int lireNbChamps() const {
		return nbDataImg;
	}

	_declspec(dllexport) double lireChamp(int i) const {
		return dataFromImg.at(i);
	}

	_declspec(dllexport) std::string lireObject(int i) const {
		return dataObject.at(i);
	}

	_declspec(dllexport) CImageCouleur* imgData() const {
		return imgPt;
	}

	_declspec(dllexport) void ecrireChamp(int i, double val) {
		dataFromImg.at(i) = val;
	}

	_declspec(dllexport) void ecrireObject(int i, const std::string& val) {
		dataObject.at(i) = val;
	}

	_declspec(dllexport) void copyImage(CImageNdg img);
	_declspec(dllexport) void writeImage(ClibIHM* img, CImageCouleur out);
	_declspec(dllexport) void writeBinaryImage(CImageNdg img);

	_declspec(dllexport) CImageNdg toBinaire();

	_declspec(dllexport) void filter(std::string methode, int kernel, std::string str);
	_declspec(dllexport) void runProcess(ClibIHM* pImgGt);

	_declspec(dllexport) void runProcessCap();

	_declspec(dllexport) void compare(ClibIHM* pImgGt);
	_declspec(dllexport) void score(ClibIHM* pImgGt);

	_declspec(dllexport) void persitData(CImageNdg* pImg, COULEUR couleur);

private:


	// Méthodes d'étiquetage
	void connectedComponentLabeling(CImageNdg& binaryImg, std::vector<std::vector<int>>& labels);

	// Méthodes d'extraction
	std::vector<Bouchon> extractBouchons(const std::vector<std::vector<int>>& labels);

	// Méthodes d'analyse des formes
	bool isCircle(const Bouchon& bouchon);
	std::string determineShape(const Bouchon& bouchon);
	bool isHeart(const Bouchon& bouchon);
	std::string determineShapeAdvanced(const Bouchon& bouchon);

	// Méthodes d'analyse des couleurs
	std::string determineColor(const Bouchon& bouchon);
	void analyzeColors(std::vector<Bouchon>& bouchons);

	// Méthodes utilitaires
	int findRoot(int label, std::map<int, Label>& labels) {
		while (labels[label].parent != label) {
			labels[label].parent = labels[labels[label].parent].parent; // Path compression
			label = labels[label].parent;
		}
		return label;
	}
};


// Fonctions d'exportation
// Pour créer un objet de la classe ClibIHM sans paramètres
extern "C" _declspec(dllexport) ClibIHM* objetLib()
{
	ClibIHM* pImg = new ClibIHM();
	return pImg;
}

// Pour créer un objet de la classe ClibIHM avec paramètres
extern "C" _declspec(dllexport) ClibIHM* objetLibDataImg(int nbChamps, byte* data, int stride, int nbLig, int nbCol)
{
	ClibIHM* pImg = new ClibIHM(nbChamps, data, stride, nbLig, nbCol);
	return pImg;
}

// Pour filtrer une image
extern "C" _declspec(dllexport) ClibIHM* filter(ClibIHM* pImg, int kernel, char* methode, char* str)
{
	if (pImg == nullptr)
		return nullptr;

	pImg->filter(methode, kernel, str);
	return pImg;
}

// Pour traiter une image
extern "C" _declspec(dllexport) ClibIHM* process(ClibIHM* pImg, ClibIHM* pImgGt)
{
	if (pImg == nullptr || pImgGt == nullptr)
		return nullptr;

	pImg->runProcess(pImgGt);
	return pImgGt;
}

extern "C" _declspec(dllexport) ClibIHM * processCap(ClibIHM * pImg)
{
	if (pImg == nullptr)
		return nullptr;

	pImg->runProcessCap();
	return pImg;
}

// Pour accéder à la valeur d'un champ
extern "C" _declspec(dllexport) double valeurChamp(ClibIHM* pImg, int i)
{
	if (pImg == nullptr)
		return 0.0;

	return pImg->lireChamp(i);
}

extern "C" _declspec(dllexport) const char* valeurObject(ClibIHM* pImg, int i)
{
	if (pImg == nullptr)
		return "";

	return pImg->lireObject(i).c_str();
}

//vider la mémoire 
extern "C" _declspec(dllexport) void destroyClibIHM(ClibIHM * pImg)
{
	if (pImg != nullptr)
	{
		delete pImg;
	}
}