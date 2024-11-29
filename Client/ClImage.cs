using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClImage : IDisposable
    {
        public IntPtr ClPtr;

        public ClImage()
        {
            ClPtr = IntPtr.Zero;
        }

        ~ClImage()
        {
            Dispose(false);
        }

        // Importer les fonctions de la bibliothèque C++
        [DllImport("libImage.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr objetLib();

        [DllImport("libImage.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr objetLibDataImg(int nbChamps, IntPtr data, int stride, int nbLig, int nbCol);

        [DllImport("libImage.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr filter(IntPtr pImg, int kernel, string methode, string str);

        [DllImport("libImage.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr process(IntPtr pImg, IntPtr pImgGt);

        [DllImport("libImage.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double valeurChamp(IntPtr pImg, int i);

        [DllImport("libImage.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void destroyClibIHM(IntPtr pImg);

        // Méthodes Wrapper
        public IntPtr ObjetLibPtr()
        {
            ClPtr = objetLib();
            return ClPtr;
        }

        public IntPtr ObjetLibDataImgPtr(int nbChamps, IntPtr data, int stride, int nbLig, int nbCol)
        {
            ClPtr = objetLibDataImg(nbChamps, data, stride, nbLig, nbCol);
            return ClPtr;
        }

        public IntPtr FilterPtr(int size, string methode, string str)
        {
            if (ClPtr == IntPtr.Zero)
                throw new InvalidOperationException("ClibIHM object is not initialized.");

            return filter(ClPtr, size, methode, str);
        }

        public IntPtr ProcessPtr(IntPtr pImgGt)
        {
            if (ClPtr == IntPtr.Zero)
                throw new InvalidOperationException("ClibIHM object is not initialized.");

            return process(ClPtr, pImgGt);
        }

        public double ObjetLibValeurChamp(int i)
        {
            if (ClPtr == IntPtr.Zero)
                throw new InvalidOperationException("ClibIHM object is not initialized.");

            return valeurChamp(ClPtr, i);
        }

        // Implémentation de IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (ClPtr != IntPtr.Zero)
            {
                destroyClibIHM(ClPtr);
                ClPtr = IntPtr.Zero;
            }
        }
    }
}
