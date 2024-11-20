#include <iostream>
#include <fstream>
#include <string>
#include <windows.h>
#include <cmath>
#include <vector>
#include <algorithm>

#include "ImageNdg.h"

#define MAGIC_NUMBER_BMP ('B'+('M'<<8)) // signature bitmap windows

// constructeurs et destructeur
CImageNdg::CImageNdg() {

	this->m_iHauteur  = 0;
	this->m_iLargeur  = 0;
	this->m_bBinaire  = false;
	this->m_sNom      = "vide";
	
	this->m_pucPixel  = NULL;
	this->m_pucPalette = NULL; 
}

CImageNdg::CImageNdg(int hauteur, int largeur, int valeur) {

	this->m_iHauteur = hauteur;
	this->m_iLargeur = largeur;
	this->m_bBinaire	= false; // Image Ndg par défaut, binaire après seuillage
	this->m_sNom      = "inconnu";

	this->m_pucPixel = new unsigned char[hauteur*largeur];
	this->m_pucPalette = new unsigned char[256*4];	
	choixPalette("grise"); // palette grise par défaut, choix utilisateur 
	if (valeur != -1) 
		for (int i=0;i<this->lireNbPixels();i++)
			this->m_pucPixel[i] = valeur;
}

CImageNdg::CImageNdg(const std::string& name) {

	BITMAPFILEHEADER header;
	BITMAPINFOHEADER infoHeader;
	
	std::ifstream f(name.c_str(),std::ios::in | std::ios::binary); 
		if (f.is_open()) {
			f.read((char*)&header,sizeof(BITMAPFILEHEADER));
			if (header.bfType != MAGIC_NUMBER_BMP) 
				throw std::string("ouverture format BMP impossible ..."); 
			else {
				f.read((char*)&infoHeader,sizeof(BITMAPINFOHEADER));
				if (infoHeader.biCompression > 0) 
					throw std::string("Format compresse non supporte...");
				else {
					if (infoHeader.biBitCount == 8) {
						this->m_iHauteur = infoHeader.biHeight;
						this->m_iLargeur = infoHeader.biWidth;
						this->m_bBinaire = false;
						this->m_sNom.assign(name.begin(),name.end()-4);
						this->m_pucPalette = new unsigned char[256*4];	
						this->m_pucPixel = new unsigned char[infoHeader.biHeight * infoHeader.biWidth];

						// gérer multiple de 32 bits via zéros éventuels ignorés
						int complement = (((this->m_iLargeur-1)/4) + 1)*4 - this->m_iLargeur;
						for (int indice=0;indice<4*256;indice++) 
							f.read((char*)&this->m_pucPalette[indice],sizeof(char));

						for (int i= this->m_iHauteur-1; i >= 0; i--) {
							for (int j=0; j<this->m_iLargeur; j++) 
								f.read((char*)&this->m_pucPixel[i*this->m_iLargeur+j],sizeof(char));

							char inutile;
							for (int k=0; k< complement; k++)
								f.read((char*)&inutile,sizeof(char));
						}
					}	
					else {
						// cas d'une image couleur
						this->m_iHauteur = infoHeader.biHeight;
						this->m_iLargeur = infoHeader.biWidth;
						this->m_bBinaire = false;
						this->m_sNom.assign(name.begin(),name.end()-4);
						this->m_pucPalette = new unsigned char[256*4];	
						this->choixPalette("grise"); // palette grise par défaut
						this->m_pucPixel = new unsigned char[infoHeader.biHeight * infoHeader.biWidth];

						// extraction plan luminance
						int complement = (((this->m_iLargeur*3-1)/4) + 1)*4 - this->m_iLargeur*3;
						for (int i= this->m_iHauteur-1; i >= 0; i--) {
							for (int j=0;j<this->m_iLargeur*3;j+=3) {
								unsigned char rouge,vert,bleu;
								f.read((char*)&rouge,sizeof(char));
								f.read((char*)&vert,sizeof(char)); 
								f.read((char*)&bleu,sizeof(char));
								this->m_pucPixel[i*this->m_iLargeur+j/3]=(unsigned char)(((int)rouge+(int)vert+(int)bleu)/3);
							}	

							char inutile;
							for (int k=0; k< complement; k++)
								f.read((char*)&inutile,sizeof(char));
						}
					}
				}
			}
			f.close();
		}
		else
			throw std::string("ERREUR : Image absente (ou pas ici en tout cas) !");
}

CImageNdg::CImageNdg(const CImageNdg& im) {

	this->m_iHauteur = im.lireHauteur();
	this->m_iLargeur = im.lireLargeur();
	this->m_bBinaire = im.lireBinaire(); 
	this->m_sNom     = im.lireNom();
	this->m_pucPixel = NULL; 
	this->m_pucPalette = NULL;

	if (im.m_pucPalette != NULL) {
		this->m_pucPalette = new unsigned char[256*4];
		memcpy(this->m_pucPalette,im.m_pucPalette,4*256);
	}
	if (im.m_pucPixel != NULL) {
		this->m_pucPixel = new unsigned char[im.lireHauteur() * im.lireLargeur()];
		memcpy(this->m_pucPixel,im.m_pucPixel,im.lireNbPixels());
	}
}

CImageNdg::~CImageNdg() {
	if (this->m_pucPixel) {
		delete[] this->m_pucPixel;
		this->m_pucPixel = NULL;
	}

	if (this->m_pucPalette) {
		delete[] this->m_pucPalette;
		this->m_pucPalette = NULL;
	}
}

void CImageNdg::sauvegarde(const std::string& fixe) {
	BITMAPFILEHEADER header;
	BITMAPINFOHEADER infoHeader;

	if (this->m_pucPixel) {
		std::string nomFichier = "../Res/";
		if (fixe.compare("") == 0)
			nomFichier += this->lireNom() + ".bmp"; // force sauvegarde dans répertoire Res (doit exister)
		else
			nomFichier += fixe;

		std::ofstream f(nomFichier.c_str(),std::ios::binary);
		if (f.is_open()) {

			int complement = (((this->m_iLargeur-1)/4) + 1)*4 - this->m_iLargeur;

			header.bfType = MAGIC_NUMBER_BMP;
			header.bfOffBits = sizeof(BITMAPFILEHEADER) + sizeof(BITMAPINFOHEADER) + 4*256*sizeof(char); // palette
			header.bfSize = header.bfOffBits + (complement+this->lireNbPixels()*sizeof(char));
			header.bfReserved1 = 0;
			header.bfReserved2 = 0;
			f.write((char*)&header,sizeof(BITMAPFILEHEADER));
			
			infoHeader.biHeight = this->m_iHauteur;
			infoHeader.biWidth = this->m_iLargeur;
			infoHeader.biCompression = 0;
			infoHeader.biClrUsed = 0;
			infoHeader.biBitCount = 8;
			infoHeader.biSizeImage = 0; //pas de compression;
			infoHeader.biClrUsed = 256;
			infoHeader.biClrImportant = 256;
			infoHeader.biHeight = this->m_iHauteur;
			infoHeader.biWidth = this->m_iLargeur;
			infoHeader.biPlanes = 1;
			infoHeader.biSize = sizeof(infoHeader);
			infoHeader.biSizeImage = this->lireNbPixels();
			f.write((char*)&infoHeader,sizeof(BITMAPINFOHEADER));

			// on remplit la palette
			for (int indice=0;indice<4*256;indice ++) 
				f.write((char*)&this->m_pucPalette[indice],sizeof(char)); 

			for (int i= this->m_iHauteur-1; i >= 0; i--) {
				for (int j=0;j<m_iLargeur;j++)    
					f.write((char*)&this->m_pucPixel[i*m_iLargeur+j],sizeof(char));
					
				// gérer multiple de 32 bits
				char inutile;
				for (int k=0; k< complement; k++)
					f.write((char*)&inutile,sizeof(char)); 
				}
		f.close();
		}
		else
			throw std::string("Impossible de creer le fichier de sauvegarde !");
	}
	else
		throw std::string("Pas de donnee a sauvegarder !");
}

CImageNdg& CImageNdg::operator=(const CImageNdg& im) {

	if (&im == this)
		return *this;

	this->m_iHauteur = im.lireHauteur();
	this->m_iLargeur = im.lireLargeur();
	this->m_bBinaire = im.lireBinaire(); 
	this->m_sNom     = im.lireNom();

	if (this->m_pucPixel) 
		delete[] this->m_pucPixel;
	this->m_pucPixel = new unsigned char[this->m_iHauteur * this->m_iLargeur];

	if (this->m_pucPalette)
		delete[] this->m_pucPalette;
	this->m_pucPalette = new unsigned char[256*4];

	if (im.m_pucPalette != NULL)
		memcpy(this->m_pucPalette,im.m_pucPalette,4*256);
	if (im.m_pucPixel != NULL)
		memcpy(this->m_pucPixel,im.m_pucPixel,im.lireNbPixels());

return *this;
}

// fonctionnalités histogramme 

std::vector<unsigned long> CImageNdg::histogramme(bool enregistrementCSV, int pas) {

	std::vector<unsigned long> h;

	h.resize(256/pas,0);
	for (int i=0;i<this->lireNbPixels();i++) 
		h[this->operator()(i)/pas] += 1L;

	if (enregistrementCSV) {
	 std::string fichier = "../Res/" + this->lireNom() + ".csv";
		std::ofstream f (fichier.c_str());

		if (!f.is_open())
			std::cout << "Impossible d'ouvrir le fichier en ecriture !" << std::endl;
		else {
			for (int i=0;i<(int)h.size();i++)
				f << h[i] << std::endl;
		}
		f.close();
	}

	return h;
}

// signatures globales

MOMENTS CImageNdg::signatures(const std::vector<unsigned long>& h) {

	MOMENTS globales;
	
	// min
	int i=0;
	while ((i < (int)h.size()) && (h[i] == 0))
		i++;
	globales.min = i;
		
	// max
	i=h.size()-1;
	while ((i > 0) && (h[i] == 0))
		i--;
	globales.max = i;

	// mediane

	int moitPop = this->lireNbPixels()/2;

	i=globales.min;
	int somme = h[i];
	while (somme < moitPop) {
		i += 1;
		if (i < (int)h.size())
			somme += h[i];
	}
	globales.mediane = i;

	// moyenne et écart-type
	double moy=0,sigma=0;
	for (i=globales.min;i<=globales.max;i++) {
		moy += ((double)h[i])*i;
		sigma += ((double)h[i])*i*i;
	}
	moy /= (double)this->lireNbPixels();
	sigma = sqrt(sigma/(double)this->lireNbPixels() - (moy*moy));
	globales.moyenne = moy;
	globales.ecartType = sigma;

	return globales;
}

MOMENTS CImageNdg::signatures() {
	
	MOMENTS globales;
	std::vector<unsigned long> hist;
	hist=this->histogramme();

	globales = this->signatures(hist);
	return globales;
}

// opérations ensemblistes images binaires
CImageNdg CImageNdg::operation(const CImageNdg& im, const std::string& methode) {

	if ((&im == this) || !(this->lireBinaire() && im.lireBinaire())) {
		return *this;
	}

	CImageNdg out(this->lireHauteur(), this->lireLargeur());
	out.m_bBinaire = this->lireBinaire(); 
	out.m_sNom     = this->lireNom()+"Op";
	out.choixPalette("binaire"); // palette binaire par défaut pour img binaire

	if (methode.compare("et") == 0) {
		for (int i=0;i<this->lireNbPixels();i++)
			out(i) = this->operator()(i) && im(i);
	}
	else
		if (methode.compare("ou") == 0) {
			for (int i=0;i<this->lireNbPixels();i++)
				out(i) = this->operator()(i) || im(i);
		}

return out;
}


// seuillage
CImageNdg CImageNdg::seuillage(const std::string& methode, int& seuilBas, int& seuilHaut) {
	
	if (!this->m_bBinaire) {
		CImageNdg out(this->lireHauteur(),this->lireLargeur());
		out.m_sNom     = this->lireNom()+"S";
		out.choixPalette("binaire"); // palette binaire par défaut
		out.m_bBinaire = true;
		seuilBas = 128;
		seuilHaut = 255;

		// création lut pour optimisation calcul
		std::vector<int> lut;
		lut.resize(256);

		// recherche valeur seuil
		// cas "manuel" -> seuil reste celui passé en paramètre

		if (methode.compare("otsu") == 0) 
		{
			std::vector<unsigned long> hist = this->histogramme();
			std::vector<unsigned long> histC; // histogramme cumulé
			histC.resize(256,0);
			histC[0] = hist[0];
			for (int i=1;i<(int)hist.size();i++) 
				histC[i] = histC[i-1]+hist[i];

			MOMENTS globales = this->signatures(hist);
			int min = globales.min,
				max = globales.max;

			// f(s)
			std::vector<double> tab;
			tab.resize(256,0);
		
			double M1, M2, w1;

			// initialisation
			M1 = min;
			seuilBas = min;
			seuilHaut = 255;

			w1 = (double)histC[min] / (double)(this->lireNbPixels());
			M2 = 0;
			for (int i = min + 1; i <= max; i++)
				M2 += (double)hist[i] * i;
			M2 /= (double)(histC[max] - hist[min]);
			tab[min] = w1 * (1 - w1) * (M1 - M2) * (M1 - M2);

			for (int i = min + 1; i < max; i++) {
				M1 = ((double)histC[i - 1] * M1 + (double)hist[i] * i) / histC[i];
				M2 = ((double)(histC[255] - histC[i - 1]) * M2 - hist[i] * i) / (double)(histC[255] - histC[i]);
				w1 = (double)histC[i] / (double)(this->lireNbPixels());
				tab[i] = w1 * (1 - w1) * (M1 - M2) * (M1 - M2);
				if (tab[i] > tab[seuilBas])
					seuilBas = i;
			}
		}

		// fin recherche valeur seuil 

		// génération lut
		for (int i = 0; i < seuilBas; i++)
			lut[i] =  0; 
		for (int i = seuilBas; i <= seuilHaut; i++)
			lut[i] = 1;
		for (int i = seuilHaut+1; i <= 255; i++)
			lut[i] = 0;

		// création image seuillée
		std::cout << "Seuillage des pixels entre " << seuilBas << " et " << seuilHaut << std::endl;
		for (int i=0; i < out.lireNbPixels(); i++) 
			out(i) = lut[this->operator ()(i)]; 

		return out;
		}
	else {
		std::cout << "Seuillage image binaire impossible" << std::endl;
		return (*this);
	}
}

// transformation

CImageNdg CImageNdg::transformation(const std::string& methode,int vMinOut, int vMaxOut) {

	CImageNdg out(this->lireHauteur(),this->lireLargeur());
	out.m_sNom     = this->lireNom()+"T";
	out.choixPalette(this->lirePalette()); // conservation de la palette
	out.m_bBinaire = this->m_bBinaire; // conservation du type

	if (methode.compare("complement") == 0) {
		if (!this->m_bBinaire) {
			// ndg -> 255-ndg
			// création lut pour optimisation calcul
			std::vector<int> lut;
			lut.resize(256);

			for (int i=0; i < 256; i++)
				lut[i] = (int)(255 - i);
			for (int i=0; i < out.lireNbPixels(); i++) 
				out(i) = lut[this->operator()(i)]; 
		}
		else {
			// 0 -> 1 et 1 -> 0
			for (int i=0; i < out.lireNbPixels(); i++) 
				out(i) = !this->operator()(i);
		}
	}
	else
		if (methode.compare("expansion") == 0) {
			std::vector<unsigned long> h = this->histogramme(false); 
			// recherche min et max image
			int min = 0,max = 255;
			int i=0;
			while (h[i] == 0)
				i++;
			min = i;
			i = 255;
			while (h[i] == 0)
				i--;
			max = i;
	
			if (max > min) {	
				double a=(double)(vMaxOut-vMinOut)/(double)(max-min);
				double b=(double)vMinOut-a*min;

				std::vector<int> lut;
				lut.resize(256);

				for (int i=min; i<=max; i++)
					lut[i] = (int)(a*i+b);

				std::cout << "Expansion dynamique [" << min << " - " << max << "] vers [" << vMinOut << " - " << vMaxOut << "]" << std::endl;
				for (int i=0; i < out.lireNbPixels(); i++) 
					out(i) = lut[this->operator()(i)]; 
			}
			else 
				for (i=0;i<this->lireNbPixels();i++)
					out(i) = this->operator()(i);
		}
		else
			if (methode.compare("egalisation") == 0) {
					std::vector<unsigned long> h = this->histogramme(false); 
					std::vector<unsigned long> hC = h;
					for (int i=1;i<(int)h.size();i++)
						hC[i] = hC[i-1] + h[i];
					
					// recherche min et max image
					int min = 0,max = 255;
					int i=0;
					while (h[i] == 0)
						i++;
					min = i;
					i = 255;
					while (h[i] == 0)
						i--;
					max = i;

					std::vector<int> lut;
					lut.resize(256);

					for (int i=min; i<=max; i++)
						lut[i] = (int)( ((double)hC[i] / (double)this->lireNbPixels() )*(double)255 );

					std::cout << "Egalisation histogramme sur [" << min << " - " << max << "] vers [0 - 255]" << std::endl;
					for (int i=0; i < out.lireNbPixels(); i++) 
						out(i) = lut[this->operator()(i)]; 
			}

	return out;
}

CImageNdg CImageNdg::difference(const CImageNdg& im) const {

	CImageNdg out(this->lireHauteur(),this->lireLargeur());
	out.m_sNom     = this->lireNom()+"D";
	out.choixPalette(this->lirePalette()); // conservation de la palette
	out.m_bBinaire = this->m_bBinaire; // conservation du type

	if (this->lireHauteur() == im.lireHauteur() && this->lireLargeur() == im.lireLargeur()) {
		for (int i=0;i<this->lireNbPixels();i++)
			out(i) = abs(this->operator()(i) - im(i));
	}
	else
		std::cout << "ERREUR : Images de tailles differentes !" << std::endl;

	return out;
}

// morphologie

CImageNdg CImageNdg::morphologie(const std::string& methode, const std::string& eltStructurant, int taille) {

	CImageNdg out(this->lireHauteur(), this->lireLargeur());
	out.m_sNom = this->lireNom() + "M";
	out.choixPalette(this->lirePalette()); // conservation de la palette
	out.m_bBinaire = this->m_bBinaire; // conservation du type

	if (methode.compare("erosion") == 0) {
		out.m_sNom = this->lireNom() + "MEr";
		CImageNdg agrandie(this->lireHauteur() + 2, this->lireLargeur() + 2);

		// gestion des bords
		if (this->lireBinaire()) {
			int pix;

			for (pix = 0; pix < agrandie.lireLargeur(); pix++) {
				agrandie(0, pix) = 1;
				agrandie(this->lireHauteur() - 1, pix) = 1;
			}
			for (pix = 1; pix < agrandie.lireHauteur() - 1; pix++) {
				agrandie(pix, 0) = 1;
				agrandie(pix, this->lireLargeur() - 1) = 1;
			}
		}
		else {
			int pix;

			for (pix = 0; pix < agrandie.lireLargeur(); pix++) {
				agrandie(0, pix) = 255;
				agrandie(this->lireHauteur() - 1, pix) = 255;
			}
			for (pix = 1; pix < agrandie.lireHauteur() - 1; pix++) {
				agrandie(pix, 0) = 255;
				agrandie(pix, this->lireLargeur() - 1) = 255;
			}
		}

		// gestion du coeur
		for (int i = 0; i < this->lireHauteur(); i++)
			for (int j = 0; j < this->lireLargeur(); j++) {
				agrandie(i + 1, j + 1) = this->operator()(i, j);
			}

		if (eltStructurant.compare("V4") == 0) {
			for (int i = 1; i < agrandie.lireHauteur() - 1; i++)
				for (int j = 1; j < agrandie.lireLargeur() - 1; j++) {
					int minH = min(agrandie(i, j - 1), agrandie(i, j + 1));
					int minV = min(agrandie(i - 1, j), agrandie(i + 1, j));
					int minV4 = min(minH, minV);
					out(i - 1, j - 1) = min(minV4, (int)agrandie(i, j));
				}
		}
		else if (eltStructurant.compare("V8") == 0) {
			for (int i = 1; i < agrandie.lireHauteur() - 1; i++) {
				for (int j = 1; j < agrandie.lireLargeur() - 1; j++) {
					int minH = min(agrandie(i, j - 1), agrandie(i, j + 1));
					int minV = min(agrandie(i - 1, j), agrandie(i + 1, j));
					int minV4 = min(minH, minV);
					int minD1 = min(agrandie(i - 1, j - 1), agrandie(i + 1, j + 1));
					int minD2 = min(agrandie(i - 1, j + 1), agrandie(i + 1, j - 1));
					int minD = min(minD1, minD2);
					int minV8 = min(minV4, minD);
					out(i - 1, j - 1) = min(minV8, (int)agrandie(i, j));
				}
			}
		}
		else if (eltStructurant.compare("disk") == 0) {
			for (int i = 1; i < agrandie.lireHauteur() - 1; i++) {
				for (int j = 1; j < agrandie.lireLargeur() - 1; j++) {
					int valeurMin = 255; // Supposons une image en niveaux de gris; ajustez selon le cas
					for (int di = -taille; di <= taille; di++) {
						for (int dj = -taille; dj <= taille; dj++) {
							if (di * di + dj * dj <= taille * taille) { // Vérification de l'appartenance au disque
								int ni = i + di; // Nouvelle coordonnée i
								int nj = j + dj; // Nouvelle coordonnée j
								// Assurez-vous que ni et nj sont dans les limites de l'image
								if (ni >= 1 && ni < agrandie.lireHauteur() - 1 && nj >= 1 && nj < agrandie.lireLargeur() - 1) {
									valeurMin = min(valeurMin, (int)agrandie(ni, nj));
								}
							}
						}
					}
					out(i - 1, j - 1) = valeurMin;
				}
			}
		}
		else if (eltStructurant.compare("full") == 0) {
			for (int i = 1; i < agrandie.lireHauteur() - 1; i++) {
				for (int j = 1; j < agrandie.lireLargeur() - 1; j++) {
					int valeurMin = 255; // Supposons une image en niveaux de gris; ajustez selon le cas
					for (int di = -taille; di <= taille; di++) {
						for (int dj = -taille; dj <= taille; dj++) {
							int ni = i + di; // Nouvelle coordonnée i
							int nj = j + dj; // Nouvelle coordonnée j
							// Assurez-vous que ni et nj sont dans les limites de l'image
							if (ni >= 1 && ni < agrandie.lireHauteur() - 1 && nj >= 1 && nj < agrandie.lireLargeur() - 1) {
								valeurMin = min(valeurMin, (int)agrandie(ni, nj));
							}
						}
					}
					out(i - 1, j - 1) = valeurMin;
				}
			}
		}
	}
	else if (methode.compare("dilatation") == 0) {
		out.m_sNom = this->lireNom() + "MDi";
		CImageNdg agrandie(this->lireHauteur() + 2, this->lireLargeur() + 2);

		// gestion des bords
		int pix;

		for (pix = 0; pix < agrandie.lireLargeur(); pix++) {
			agrandie(0, pix) = 0;
			agrandie(agrandie.lireHauteur() - 1, pix) = 0;
		}
		for (pix = 1; pix < agrandie.lireHauteur() - 1; pix++) {
			agrandie(pix, 0) = 0;
			agrandie(pix, agrandie.lireLargeur() - 1) = 0;
		}

		// gestion du coeur
		for (int i = 0; i < this->lireHauteur(); i++)
			for (int j = 0; j < this->lireLargeur(); j++) {
				agrandie(i + 1, j + 1) = this->operator()(i, j);
			}

		if (eltStructurant.compare("V4") == 0) {
			for (int i = 1; i < agrandie.lireHauteur() - 1; i++)
				for (int j = 1; j < agrandie.lireLargeur() - 1; j++) {
					int maxH = max(agrandie(i, j - 1), agrandie(i, j + 1));
					int maxV = max(agrandie(i - 1, j), agrandie(i + 1, j));
					int maxV4 = max(maxH, maxV);
					out(i - 1, j - 1) = max(maxV4, (int)agrandie(i, j));
				}
		}
		else if (eltStructurant.compare("V8") == 0) {
			for (int i = 1; i < agrandie.lireHauteur() - 1; i++) {
				for (int j = 1; j < agrandie.lireLargeur() - 1; j++) {
					int maxH = max(agrandie(i, j - 1), agrandie(i, j + 1));
					int maxV = max(agrandie(i - 1, j), agrandie(i + 1, j));
					int maxV4 = max(maxH, maxV);
					int maxD1 = max(agrandie(i - 1, j - 1), agrandie(i + 1, j + 1));
					int maxD2 = max(agrandie(i - 1, j + 1), agrandie(i + 1, j - 1));
					int maxD = max(maxD1, maxD2);
					int maxV8 = max(maxV4, maxD);
					out(i - 1, j - 1) = max(maxV8, (int)agrandie(i, j));
				}
			}
		}
		else if (eltStructurant.compare("disk") == 0) {
			for (int i = 1; i < agrandie.lireHauteur() - 1; i++) {
				for (int j = 1; j < agrandie.lireLargeur() - 1; j++) {
					int valeurMax = 0; // Supposons une image en niveaux de gris; ajustez selon le cas
					for (int di = -taille; di <= taille; di++) {
						for (int dj = -taille; dj <= taille; dj++) {
							if (di * di + dj * dj <= taille * taille) { // Vérification de l'appartenance au disque
								int ni = i + di; // Nouvelle coordonnée i
								int nj = j + dj; // Nouvelle coordonnée j
								// Assurez-vous que ni et nj sont dans les limites de l'image
								if (ni >= 1 && ni < agrandie.lireHauteur() - 1 && nj >= 1 && nj < agrandie.lireLargeur() - 1) {
									valeurMax = max(valeurMax, (int)agrandie(ni, nj));
								}
							}
						}
					}
					out(i - 1, j - 1) = valeurMax;
				}
			}
		}
		else if (eltStructurant.compare("full") == 0) {
			for (int i = 1; i < agrandie.lireHauteur() - 1; i++) {
				for (int j = 1; j < agrandie.lireLargeur() - 1; j++) {
					int valeurMax = 0; // Supposons une image en niveaux de gris; ajustez selon le cas
					for (int di = -taille; di <= taille; di++) {
						for (int dj = -taille; dj <= taille; dj++) {
							int ni = i + di; // Nouvelle coordonnée i
							int nj = j + dj; // Nouvelle coordonnée j
							// Assurez-vous que ni et nj sont dans les limites de l'image
							if (ni >= 1 && ni < agrandie.lireHauteur() - 1 && nj >= 1 && nj < agrandie.lireLargeur() - 1) {
								valeurMax = max(valeurMax, (int)agrandie(ni, nj));
							}
						}
					}
					out(i - 1, j - 1) = valeurMax;
				}
			}
		}
	}
	return out;
}

CImageNdg CImageNdg::ouverture(const std::string& eltStructurant, int taille) {

	CImageNdg out = this->morphologie("erosion", eltStructurant, taille);
	out = out.morphologie("dilatation", eltStructurant, taille);

	return out;
}

CImageNdg CImageNdg::fermeture(const std::string& eltStructurant, int taille) {

	CImageNdg out = this->morphologie("dilatation", eltStructurant, taille);
	out = out.morphologie("erosion", eltStructurant, taille);

	return out;
}

CImageNdg CImageNdg::whiteTopHat(const std::string& strel, int taille) {
	CImageNdg out = this->ouverture(strel, taille);
	out = this->difference(out);
	return out;
}

CImageNdg CImageNdg::blackTopHat(const std::string& strel, int taille) {
	CImageNdg out = this->fermeture(strel, taille);
	out = this->difference(out);
	return out;
}

double CImageNdg::indicateurPerformance(const CImageNdg& GroundTruth, const std::string& methode) const {

	if (this->lireHauteur() != GroundTruth.lireHauteur() || this->lireLargeur() != GroundTruth.lireLargeur()) {
		throw std::string("Les images doivent avoir la m�me taille");
		return -1;
	}

	if (!this->lireBinaire() || !GroundTruth.lireBinaire()) {
		throw std::string("Les images doivent �tre binaires");
		return -1;
	}

	double res = 0;

	if (methode.compare("iou") == 0) {
		int cardInter = 0, cardUnion = 0;

		for (int i = 0; i < this->lireNbPixels(); i++) {
			if (this->operator()(i) == 1 && GroundTruth(i) == 1) {
				cardInter++;
			}
			if (this->operator()(i) == 1 || GroundTruth(i) == 1) {
				cardUnion++;
			}
		}
		res = (float)cardInter / cardUnion;
	}
	else if (methode.compare("precision") == 0) {
		double cardInter = 0, cardResult = 0;

		for (int i = 0; i < this->lireNbPixels(); i++) {
			if (this->operator()(i) == 1 && GroundTruth(i) == 1) {
				cardInter++;
			}
			if (this->operator()(i) == 1) {
				cardResult++;
			}
		}
		res = cardInter / cardResult;

	}
	else if (methode.compare("rappel") == 0) {
		double cardInter = 0, cardGroundTruth = 0;

		for (int i = 0; i < this->lireNbPixels(); i++) {
			if (this->operator()(i) == 1 && GroundTruth(i) == 1) {
				cardInter++;
			}
			if (GroundTruth(i) == 1) {
				cardGroundTruth++;
			}
		}
		res = cardInter / cardGroundTruth;
	}
	else if (methode.compare("f-meas") == 0) {
		double cardInter = 0, cardGroundTruth = 0, cardResult = 0;

		for (int i = 0; i < this->lireNbPixels(); i++) {
			if (this->operator()(i) == 1 && GroundTruth(i) == 1) {
				cardInter++;
			}
			if (GroundTruth(i) == 1) {
				cardGroundTruth++;
			}
			if (this->operator()(i) == 1) {
				cardResult++;
			}
		}
		res = 2 * cardInter / (cardGroundTruth + cardResult);
	}
	else if (methode.compare("jaccard") == 0) {
		double cardInter = 0, cardUnion = 0;

		for (int i = 0; i < this->lireNbPixels(); i++) {
			if (this->operator()(i) == 1 && GroundTruth(i) == 1) {
				cardInter++;
			}
			if (this->operator()(i) == 1 || GroundTruth(i) == 1) {
				cardUnion++;
			}
		}
		res = cardInter / (cardUnion - cardInter);
	}
	else {
		throw std::string("Methode non reconnue");
		return -1;
	}

	return res;
}

double CImageNdg::correlation(const CImageNdg& GroundTruth) const {

	double sum_A = 0, sum_B = 0;
	double mean_A, mean_B;
	double numerator = 0, denominator_A = 0, denominator_B = 0;
	double correlation;

	for (int i = 0; i < this->lireNbPixels(); i++) {
		sum_A += this->operator()(i);
		sum_B += GroundTruth(i);
	}

	mean_A = sum_A / this->lireNbPixels();
	mean_B = sum_B / this->lireNbPixels();

	for (int i = 0; i < this->lireNbPixels(); i++) {
		numerator += (this->operator()(i) - mean_A) * (GroundTruth(i) - mean_B);
		denominator_A += pow(this->operator()(i) - mean_A, 2);
		denominator_B += pow(GroundTruth(i) - mean_B, 2);
	}

	return (numerator / sqrt(denominator_A * denominator_B));
}

// filtrage
CImageNdg CImageNdg::filtrage(const std::string& methode, int Ni, int Nj, const std::string& str) {
		
	CImageNdg out(this->lireHauteur(),this->lireLargeur());
	out.m_sNom     = this->lireNom()+"F";
	out.choixPalette(this->lirePalette()); // conservation de la palette
	out.m_bBinaire = this->m_bBinaire; // conservation du type
		
	if (methode.compare("moyennage") == 0) {
		if (str.compare("disk") == 0)
		{
			if (Ni == Nj && Ni % 2 == 1) { // Vérification que Ni et Nj sont impairs pour former un disque
				int rayon = Ni / 2;
				for (int i = 0; i < this->lireHauteur(); i++) {
					for (int j = 0; j < this->lireLargeur(); j++) {
						float somme = 0;
						float moy = 0;
						for (int di = -rayon; di <= rayon; di++) {
							for (int dj = -rayon; dj <= rayon; dj++) {
								if (di * di + dj * dj <= rayon * rayon) {
									int k = i + di;
									int l = j + dj;
									if (k >= 0 && k < this->lireHauteur() && l >= 0 && l < this->lireLargeur()) {
										moy += (float)this->operator()(k, l);
										somme += 1;
									}
								}
							}
						}
						out(i, j) = (int)(moy / somme);
					}
				}
			}
		} else if (str.compare("carre")==0)
		{
			int nbBordsi = Ni / 2;
			int nbBordsj = Nj / 2;

			for (int i = 0; i < this->lireHauteur(); i++)
				for (int j = 0; j < this->lireLargeur(); j++) {
					// gestion des bords
					int dk = max(0, i - nbBordsi);
					int fk = min(i + nbBordsi, this->lireHauteur() - 1);
					int dl = max(0, j - nbBordsj);
					int fl = min(j + nbBordsj, this->lireLargeur() - 1);

					float somme = 0;
					float moy = 0;
					for (int k = dk; k <= fk; k++)
						for (int l = dl; l <= fl; l++) {
							moy += (float)this->operator()(k, l);
							somme += 1;
						}
					out(i, j) = (int)(moy / somme);
				}
		}
		else {
			int nbBordsi = Ni / 2;
			int nbBordsj = Nj / 2;

			for (int i = 0; i < this->lireHauteur(); i++)
				for (int j = 0; j < this->lireLargeur(); j++) {
					// gestion des bords
					int dk = max(0, i - nbBordsi);
					int fk = min(i + nbBordsi, this->lireHauteur() - 1);
					int dl = max(0, j - nbBordsj);
					int fl = min(j + nbBordsj, this->lireLargeur() - 1);

					float somme = 0;
					float moy = 0;
					for (int k = dk; k <= fk; k++)
						for (int l = dl; l <= fl; l++) {
							moy += (float)this->operator()(k, l);
							somme += 1;
						}
					out(i, j) = (int)(moy / somme);
				}
		}
	}
	else {
		if (methode.compare("median") == 0) {

			if (str.compare("disk") == 0)
			{
				if (Ni == Nj && Ni % 2 == 1) {
					int rayon = Ni / 2;
					std::vector<int> voisinage;

					for (int i = 0; i < this->lireHauteur(); i++) {
						for (int j = 0; j < this->lireLargeur(); j++) {
							voisinage.clear(); // Réinitialisez le vecteur pour chaque pixel

							for (int di = -rayon; di <= rayon; di++) {
								for (int dj = -rayon; dj <= rayon; dj++) {

									if (di * di + dj * dj <= rayon * rayon) {
										int k = i + di;
										int l = j + dj;
										if (k >= 0 && k < this->lireHauteur() && l >= 0 && l < this->lireLargeur()) {
											voisinage.push_back(this->operator()(k, l)); // Ajoutez la valeur du pixel voisin au vecteur
										}
									}
								}
							}

							// Triez le vecteur pour trouver la médiane
							std::sort(voisinage.begin(), voisinage.end());

							// Assignez la médiane au pixel de sortie								
							out(i, j) = voisinage[voisinage.size() / 2];
						}
					}
				}
			} else if (str.compare("carre") == 0)
			{
				int nbBordsi = Ni / 2;
				int nbBordsj = Nj / 2;

				std::vector<int> voisinage;

				for (int i = 0; i < this->lireHauteur(); i++)
				{
					for (int j = 0; j < this->lireLargeur(); j++)
					{
						// gestion des bords
						int dk = max(0, i - nbBordsi);
						int fk = min(i + nbBordsi, this->lireHauteur() - 1);
						int dl = max(0, j - nbBordsj);
						int fl = min(j + nbBordsj, this->lireLargeur() - 1);

						voisinage.resize((fk - dk + 1) * (fl - dl + 1));
						int indMed = (fk - dk + 1) * (fl - dl + 1) / 2;

						// empilement 
						int indice = 0;
						for (int k = dk; k <= fk; k++)
							for (int l = dl; l <= fl; l++)
							{
								voisinage.at(indice) = (int)this->operator()(k, l);
								indice++;
							}

						// tri croissant
						std::sort(voisinage.begin(), voisinage.end());

						out(i, j) = voisinage.at(indMed);

						voisinage.clear();
					}
				}
			}
			else {
				int nbBordsi = Ni / 2;
				int nbBordsj = Nj / 2;

				std::vector<int> voisinage;

				for (int i = 0; i < this->lireHauteur(); i++)
					for (int j = 0; j < this->lireLargeur(); j++) {
						// gestion des bords
						int dk = max(0, i - nbBordsi);
						int fk = min(i + nbBordsi, this->lireHauteur() - 1);
						int dl = max(0, j - nbBordsj);
						int fl = min(j + nbBordsj, this->lireLargeur() - 1);

						voisinage.resize((fk - dk + 1) * (fl - dl + 1));
						int indMed = (fk - dk + 1) * (fl - dl + 1) / 2;

						// empilement 
						int indice = 0;
						for (int k = dk; k <= fk; k++)
							for (int l = dl; l <= fl; l++) {
								voisinage.at(indice) = (int)this->operator()(k, l);
								indice++;
							}

						// tri croissant
						std::sort(voisinage.begin(), voisinage.end());

						out(i, j) = voisinage.at(indMed);

						voisinage.clear();
					}
			}
		}
	}

	return out;
}


