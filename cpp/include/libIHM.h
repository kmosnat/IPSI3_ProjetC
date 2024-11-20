#pragma once

#include "ImageClasse.h"
#include "ImageNdg.h"
#include "ImageCouleur.h"
#include "ImageDouble.h"

#include <windows.h>
#include <thread>
#include <vector>

// Enumération des couleurs
enum class COULEUR
{
	RVB,
	rouge,
	vert,
	bleu
};


class ClibIHM {

	///////////////////////////////////////
private:
	///////////////////////////////////////

	// data n�cessaires � l'IHM donc fonction de l'application cibl�e
	int						nbDataImg; 
	std::vector<double>		dataFromImg; 
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

	_declspec(dllexport) CImageCouleur* imgData() const {
		return imgPt;
	}

	_declspec(dllexport) void ecrireChamp(int i, double val) {
		dataFromImg.at(i) = val;
	}

	_declspec(dllexport) void copyImage(CImageNdg img);
	_declspec(dllexport) void writeImage(ClibIHM* img, CImageCouleur out);
	_declspec(dllexport) void writeBinaryImage(CImageNdg img);

	_declspec(dllexport) CImageNdg toBinaire();

	_declspec(dllexport) void filter(std::string methode, int kernel, std::string str);
	_declspec(dllexport) void runProcess(ClibIHM* pImgGt);

	_declspec(dllexport) void compare(ClibIHM* pImgGt);
	_declspec(dllexport) void score(ClibIHM* pImgGt);

	_declspec(dllexport) void persitData(CImageNdg* pImg, COULEUR couleur);
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
extern "C" _declspec(dllexport) ClibIHM * filter(ClibIHM* pImg, int kernel, char* methode, char* str)
{
	pImg->filter(methode, kernel, str);
	return pImg;
}

// Pour traiter une image
extern "C" _declspec(dllexport) ClibIHM* process(ClibIHM* pImg, ClibIHM* pImgGt)
{
	pImg->runProcess(pImgGt);
	return pImgGt;
}

// Pour accéder à la valeur d'un champ
extern "C" _declspec(dllexport) double valeurChamp(ClibIHM* pImg, int i)
{
	return pImg->lireChamp(i);
}