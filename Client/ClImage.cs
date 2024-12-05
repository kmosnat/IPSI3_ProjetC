using System;
using System.Runtime.InteropServices;

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

        [DllImport("libImage.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr objetLib();

        [DllImport("libImage.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr objetLibDataImg(int nbChamps, IntPtr data, int stride, int nbLig, int nbCol);

        [DllImport("libImage.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr filter(IntPtr pImg, int kernel, string methode, string str);

        [DllImport("libImage.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr process(IntPtr pImg, IntPtr pImgGt);

        [DllImport("libImage.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr processCap(IntPtr pImg);

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

        public IntPtr FilterPtr(int kernel, string methode, string str)
        {
            if (ClPtr == IntPtr.Zero)
                throw new InvalidOperationException("L'objet ClibIHM n'est pas initialisé.");

            return filter(ClPtr, kernel, methode, str);
        }

        public IntPtr ProcessPtr(IntPtr pImgGt)
        {
            if (ClPtr == IntPtr.Zero)
                throw new InvalidOperationException("L'objet ClibIHM n'est pas initialisé.");

            return process(ClPtr, pImgGt);
        }

        public IntPtr ProcessCapPtr()
        {
            if (ClPtr == IntPtr.Zero)
                throw new InvalidOperationException("L'objet ClibIHM n'est pas initialisé.");

            return processCap(ClPtr);
        }

        public double ObjetLibValeurChamp(int i)
        {
            if (ClPtr == IntPtr.Zero)
                throw new InvalidOperationException("L'objet ClibIHM n'est pas initialisé.");

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
