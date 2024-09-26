using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowShop
{
    class claseSolucion
    {
        int[,] ctAux;
        int[] secuenciaSolucionAux;
        int[,] secuenciaSolucionExtendidaAux;
        int makespanAux;
        public int[,] ct
        {
            get { return ctAux; }
            set { ctAux = value; }
        }
        public int[] secuenciaSolucion
        {
            get { return secuenciaSolucionAux; }
            set { secuenciaSolucionAux = value; }
        }
        public int[,] secuenciaSolucionExtendida
        {
            get { return secuenciaSolucionExtendidaAux; }
            set { secuenciaSolucionExtendidaAux = value; }
        }
        public int makespan
        {
            get { return makespanAux; }
            set { makespanAux = value; }
        }
    }
}
