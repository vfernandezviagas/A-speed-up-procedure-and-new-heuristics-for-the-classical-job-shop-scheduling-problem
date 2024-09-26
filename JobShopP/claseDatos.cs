using System;
using System.Collections.Generic;
using System.Text;

namespace FlowShop
{
    class claseDatos
    {
        int[,] tiemposProceso_ijAux;
        int[,] Ruta_jiAux;
        int[,] Ruta_ijAux;
        int[] dueDates_jAux;
        int[] releaseTimes_jAux;
        int lowerBoundAux;
        int NAux;
        int MAux;
        public int[,] tiemposProceso_ij
        {
            get { return tiemposProceso_ijAux; }
            set { tiemposProceso_ijAux = value; }
        }
        public int[,] Ruta_ji
        {
            get { return Ruta_jiAux; }
            set { Ruta_jiAux = value; }
        }
        public int[,] Ruta_ij
        {
            get { return Ruta_ijAux; }
            set { Ruta_ijAux = value; }
        }
        public int[] dueDates_j
        {
            get { return dueDates_jAux; }
            set { dueDates_jAux = value; }
        }
        public int[] releaseTimes_j
        {
            get { return dueDates_jAux; }
            set { dueDates_jAux = value; }
        }
        public int N
        {
            get { return NAux; }
            set { NAux = value; }
        }
        public int M
        {
            get { return MAux; }
            set { MAux = value; }
        }
        public int lowerBound
        {
            get { return lowerBoundAux; }
            set { lowerBoundAux = value; }
        }

    }
}
