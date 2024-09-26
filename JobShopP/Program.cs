using System;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace FlowShop
{
    class Program
    {
        static void Main(string[] args)
        {
            claseDatos datos = new claseDatos();
            claseDatos datos_Pham = new claseDatos();
            double[] T = new double[3] { 0.2, 0.4, 0.6 };
            double[] R = new double[3] { 0.2, 0.6, 1 };
            int[] jobs = new int[] { 10, 20, 30, 40, 50 };
            int[] machines = new int[] { 5, 10, 15, 20 };
            System.IO.StreamWriter[] ficheros;
            int numero_ficheros = 5;
            nuevos_ficheros(numero_ficheros, out ficheros);
            cerrar_ficheros(ficheros);
            Random aleatorio = new Random(0);
            string[] nombresBaterias = new string[] { "abz", "dmu", "ft", "la", "orb", "swv", "ta", "yn" };
            int[] instancias_cada_bateria = new int[] { 5, 80, 3, 40, 10, 20, 80, 4 };
            int contadorG = -1;
            for (int jj = 0; jj < nombresBaterias.Length; jj++)
            {
                for (int iteracion = 1; iteracion <= instancias_cada_bateria[jj]; iteracion++)
                {
                    contadorG++;
                    abrir_ficheros(numero_ficheros, out ficheros);
                    datos = lecturaTestBed_Formato_Taillard(datos, nombresBaterias[jj], iteracion);
                    for (int i1 = 0; i1 < numero_ficheros; i1++)
                    {
                        ficheros[i1].Write(nombresBaterias[jj] + ";" + iteracion + ";" + datos.N + ";" + datos.M + ";");
                    }
                    claseSolucion solucion = new claseSolucion();
                    solucion.secuenciaSolucion = new int[datos.N * datos.M];
                    int[] contadorTrabajos = new int[datos.N];
                    for (int i = 0; i < solucion.secuenciaSolucion.Length; i++)
                    {
                        int numAleat = aleatorio.Next(datos.N);
                        while (contadorTrabajos[numAleat] >= datos.M)
                        {
                            numAleat = aleatorio.Next(datos.N);
                        }
                        solucion.secuenciaSolucion[i] = numAleat;
                        contadorTrabajos[numAleat]++;
                    }
                    var timer = Stopwatch.StartNew(); Stopwatch.StartNew(); double tiemposMs; double tiemposSeg; 
                    int CmaxMWR;
                    int beamWidth = 5;
                    int filterWidth = 5;
                    int FO;
                    int[] pi_t = new int[1]; int[] pi_m = new int[1]; claseSolucion sH1;

                    //FTP1
                    timer = Stopwatch.StartNew();
                    pi_t = new int[1]; pi_m = new int[1];
                    FTP1(datos.N, datos.M, datos.tiemposProceso_ij, datos.Ruta_ij, out pi_t, out pi_m);
                    CmaxMWR = Decoding_ObR_JS_makespan_v1(datos.M, datos.N, datos.tiemposProceso_ij, pi_t, pi_m, datos.Ruta_ij);
                    ficheros[0].Write(CmaxMWR + ";");
                    timer.Stop(); tiemposMs = timer.ElapsedMilliseconds; tiemposSeg = tiemposMs / 1000;
                    ficheros[1].Write(tiemposSeg + ";");
                    //FTP1_LS
                    timer = Stopwatch.StartNew();
                    sH1 = FTP1_LS(datos);
                    ficheros[0].Write(sH1.makespan + ";");
                    timer.Stop(); tiemposMs = timer.ElapsedMilliseconds; tiemposSeg = tiemposMs / 1000;
                    ficheros[1].Write(tiemposSeg + ";");
                    //FTP2(5)
                    timer = Stopwatch.StartNew();
                    sH1 = FTP2(datos, 5);
                    ficheros[0].Write(sH1.makespan + ";");
                    timer.Stop(); tiemposMs = timer.ElapsedMilliseconds; tiemposSeg = tiemposMs / 1000;
                    ficheros[1].Write(tiemposSeg + ";");
                    //FTP2(n)
                    timer = Stopwatch.StartNew();
                    sH1 = FTP2(datos, datos.N);
                    ficheros[0].Write(sH1.makespan + ";");
                    timer.Stop(); tiemposMs = timer.ElapsedMilliseconds; tiemposSeg = tiemposMs / 1000;
                    ficheros[1].Write(tiemposSeg + ";");
                    //BS_FTP1(5)
                    beamWidth = 5;
                    filterWidth = 5;
                    timer = Stopwatch.StartNew();
                    FO = BS_FTP1(datos, beamWidth, filterWidth);
                    ficheros[0].Write(FO + ";");
                    timer.Stop(); tiemposMs = timer.ElapsedMilliseconds; tiemposSeg = tiemposMs / 1000;
                    ficheros[1].Write(tiemposSeg + ";");
                    //BS_FTP1_LS(5)
                    beamWidth = 5;
                    filterWidth = 5;
                    timer = Stopwatch.StartNew();
                    FO = BS_FTP1_LS(datos, beamWidth, filterWidth);
                    ficheros[0].Write(FO + ";");
                    timer.Stop(); tiemposMs = timer.ElapsedMilliseconds; tiemposSeg = tiemposMs / 1000;
                    ficheros[1].Write(tiemposSeg + ";");


                    for (int i1 = 0; i1 < numero_ficheros; i1++)
                    {
                        ficheros[i1].WriteLine();
                    }
                    cerrar_ficheros(ficheros);
                }
            }
        }
        private static claseDatos lecturaTestBed_Formato_Taillard(claseDatos datos, string nombreBateria, int numInstancia)
        {
            String ruta2 = AppDomain.CurrentDomain.BaseDirectory;
            ruta2 = ruta2.Replace("\\", "/").ToString();
            String dirTemp = ruta2 + "Instancias/";
            if (nombreBateria == "abz")
            {
                numInstancia = numInstancia + 4;
            }
            else if (nombreBateria == "ft")
            {
                if (numInstancia == 1)
                {
                    numInstancia = 6;
                }
                else if (numInstancia == 2)
                {
                    numInstancia = 10;
                }
                else if (numInstancia == 3)
                {
                    numInstancia = 20;
                }
            }
            string problemaString = "" + numInstancia;
            if (nombreBateria != "abz")
            {
                if (numInstancia < 10)
                {
                    problemaString = "0" + numInstancia;
                }
            }

            String fichero = dirTemp + nombreBateria + problemaString + ".txt";
            FileInfo FILEESC = new FileInfo(fichero);
            String[] substr;
            System.IO.StreamReader sr = new System.IO.StreamReader(fichero);
            string fila = sr.ReadLine();
            char delimitador = '\t';
            substr = fila.Split(delimitador);
            string trabajos = substr[0];
            int numJobs = int.Parse(trabajos);
            string maquinas = substr[1];
            int numMachines = int.Parse(maquinas);
            datos.N = numJobs;
            datos.M = numMachines;
            datos.tiemposProceso_ij = new int[numMachines, numJobs];
            datos.Ruta_ij = new int[numMachines, numJobs];
            int[,] p_ij_Aux = new int[numMachines, numJobs];
            for (int j = 0; j < numJobs; j++)
            {
                fila = sr.ReadLine();
                substr = fila.Split(delimitador);
                for (int m = 0; m < numMachines; m++)
                {
                    p_ij_Aux[m, j] = int.Parse(substr[m]);
                }
            }
            for (int j = 0; j < numJobs; j++)
            {
                fila = sr.ReadLine();
                substr = fila.Split(delimitador);
                for (int m = 0; m < numMachines; m++)
                {
                    datos.Ruta_ij[m, j] = int.Parse(substr[m]) - 1;
                }
            }
            for (int i = 0; i < numMachines; i++)
            {
                for (int j = 0; j < numJobs; j++)
                {
                    datos.tiemposProceso_ij[datos.Ruta_ij[i, j], j] = p_ij_Aux[i, j];
                }
            }
            sr.Close();
            return datos;
        }
        private static void abrir_ficheros(int num_ficheros, out System.IO.StreamWriter[] FicheroCSV)
        {
            FicheroCSV = new System.IO.StreamWriter[num_ficheros];
            for (int i = 0; i < num_ficheros; i++)
            {
                String ruta = AppDomain.CurrentDomain.BaseDirectory;
                ruta = ruta.Replace("\\", "/").ToString();
                String ficCSV = ruta + "temp" + "/fichero_" + i + ".csv";
                FicheroCSV[i] = new System.IO.StreamWriter(ficCSV, true);
            }
        }
        private static void cerrar_ficheros(System.IO.StreamWriter[] FicheroCSV)
        {
            for (int i = 0; i < FicheroCSV.Length; i++)
            {
                FicheroCSV[i].Close();
            }
        }
        private static void nuevos_ficheros(int num_ficheros, out System.IO.StreamWriter[] FicheroCSV)
        {
            FicheroCSV = new System.IO.StreamWriter[num_ficheros];
            for (int i = 0; i < num_ficheros; i++)
            {
                String ruta = AppDomain.CurrentDomain.BaseDirectory;
                ruta = ruta.Replace("\\", "/").ToString();
                DirectoryInfo DIRESC = new DirectoryInfo(ruta + "/temp");
                if (!DIRESC.Exists)
                {
                    DIRESC.Create();
                }
                String ficBatBorrar = ruta + "temp" + "/fichero_" + i + ".csv";
                FileInfo FicheroEliminar = new FileInfo(ficBatBorrar);
                if (FicheroEliminar.Exists)
                {
                    File.Delete(ficBatBorrar);
                }
                String ficCSV = ruta + "temp" + "/fichero_" + i + ".csv";
                FicheroCSV[i] = new System.IO.StreamWriter(ficCSV, true);
                FicheroCSV[i].Write("\n\n");
            }
        }
        //Decoding
        private static int Decoding_ObR_JS_makespan_v1(int M, int numTrabajosEnSecuencia, int[,] tiemposProceso_ij, int[] solucion_trabajo, int[] solucion_maquina, int[,] Ruta)
        {
            int FO = 0;
            int[] ocupacionMaquinas = new int[M];
            int numElementos = solucion_trabajo.Length;
            int[,] tiempoFinalizacion = new int[M, numTrabajosEnSecuencia];
            int[] maq_Anterior = new int[numTrabajosEnSecuencia];
            for (int i = 0; i < M; i++)
            {
                ocupacionMaquinas[i] = 0;
            }
            for (int j = 0; j < numTrabajosEnSecuencia; j++)
            {
                maq_Anterior[j] = -1;
            }
            for (int j = 0; j < numElementos; j++)
            {
                int trab = solucion_trabajo[j];
                int maq = solucion_maquina[j];
                if (Ruta[0, trab] == maq)
                {
                    ocupacionMaquinas[maq] = ocupacionMaquinas[maq] + tiemposProceso_ij[maq, trab];
                    tiempoFinalizacion[maq, trab] = ocupacionMaquinas[maq];
                }
                else
                {
                    if (tiempoFinalizacion[maq_Anterior[trab], trab] >= ocupacionMaquinas[maq])
                    {
                        ocupacionMaquinas[maq] = tiempoFinalizacion[maq_Anterior[trab], trab] + tiemposProceso_ij[maq, trab];
                        tiempoFinalizacion[maq, trab] = ocupacionMaquinas[maq];
                    }
                    else
                    {
                        ocupacionMaquinas[maq] = ocupacionMaquinas[maq] + tiemposProceso_ij[maq, trab];
                        tiempoFinalizacion[maq, trab] = ocupacionMaquinas[maq];
                    }
                }
                maq_Anterior[trab] = maq;
            }
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < numTrabajosEnSecuencia; j++)
                {
                    if (tiempoFinalizacion[i, j] > FO)
                    {
                        FO = tiempoFinalizacion[i, j];
                    }
                }
            }
            return FO;
        }
        //Filter 
        private static int Filtro_NonDelay_Schedule(int m, int n, int[,] p_ij, int[,] Ruta_ij, int filterWidth, int num_elem_actual, int beamActual, int[,] sec_op_j, int[,] sec_op_i, int[] suma_p_j, out int[] trabajosFiltrados)
        {
            int[] resto_pj = new int[n];
            for (int j = 0; j < n; j++)
            {
                resto_pj[j] = suma_p_j[j];
            }
            int[] elementos_j = new int[n];
            int[] elementos_i = new int[m];
            int[] carga_maq = new int[m];
            int[] carga_trab = new int[n];
            for (int j = 0; j < num_elem_actual; j++)
            {
                int maq = sec_op_i[beamActual, j];
                int trab = sec_op_j[beamActual, j];
                resto_pj[trab] -= p_ij[maq, trab];
                elementos_j[trab]++;
                elementos_i[maq]++;
                int comienzo = carga_maq[maq];
                if (carga_trab[trab] > comienzo)
                {
                    comienzo = carga_trab[trab];
                }
                carga_trab[trab] = comienzo + p_ij[maq, trab];
                carga_maq[maq] = comienzo + p_ij[maq, trab];
            }
            int comienzo_min = 0;
            int maq_min = 0;
            bool PV = true;
            int numTrabajosParaFiltrar = 0;
            for (int j = 0; j < n; j++)
            {
                if (elementos_j[j] < m)
                {
                    int maq = Ruta_ij[elementos_j[j], j];
                    int trab = j;
                    int comienzo = carga_maq[maq];
                    if (carga_trab[trab] > comienzo)
                    {
                        comienzo = carga_trab[trab];
                    }
                    if (PV == true)
                    {
                        PV = false;
                        comienzo_min = comienzo;
                        maq_min = maq;
                        numTrabajosParaFiltrar = 1;
                    }
                    else if (comienzo < comienzo_min)
                    {
                        comienzo_min = comienzo;
                        maq_min = maq;
                        numTrabajosParaFiltrar = 1;
                    }
                    else if (comienzo == comienzo_min && maq == maq_min)
                    {
                        numTrabajosParaFiltrar++;
                    }
                }
            }
            int[] trabajosParaFiltrar = new int[numTrabajosParaFiltrar];
            int[] p_trabajosParaFiltrar = new int[numTrabajosParaFiltrar];
            int cont = 0;
            for (int j = 0; j < n; j++)
            {
                if (elementos_j[j] < m)
                {
                    int maq = Ruta_ij[elementos_j[j], j];
                    int trab = j;
                    int comienzo = carga_maq[maq];
                    if (carga_trab[trab] > comienzo)
                    {
                        comienzo = carga_trab[trab];
                    }
                    if (comienzo == comienzo_min && maq == maq_min)
                    {
                        trabajosParaFiltrar[cont] = trab;
                        p_trabajosParaFiltrar[cont] = resto_pj[trab];
                        cont++;
                    }
                }
            }
            int[] orden = new int[numTrabajosParaFiltrar];
            double[,] Elements = new double[2, numTrabajosParaFiltrar];
            for (int k = 0; k < numTrabajosParaFiltrar; k++)
            {
                Elements[0, k] = k;
                Elements[1, k] = p_trabajosParaFiltrar[k];
            }
            double[,] ElementsSalida = Quicksort_decre(Elements, 0, numTrabajosParaFiltrar - 1);
            for (int k = 0; k < numTrabajosParaFiltrar; k++)
            {
                orden[k] = (int)ElementsSalida[0, k];
            }
            trabajosFiltrados = new int[Math.Min(filterWidth, numTrabajosParaFiltrar)];
            for (int k = 0; k < Math.Min(filterWidth, numTrabajosParaFiltrar); k++)
            {
                trabajosFiltrados[k] = trabajosParaFiltrar[orden[k]];
            }
            return maq_min;
        }

        //***BS_FTP1
        private static int BS_FTP1(claseDatos datos, int beamWidth, int filterWidth)
        {
            int m = datos.M;
            int n = datos.N;
            int[,] p_ij = datos.tiemposProceso_ij;
            int[,] Ruta_ij = datos.Ruta_ij;
            claseSolucion solucion = new claseSolucion();
            int[] suma_p_j = new int[n];
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < m; i++)
                {
                    suma_p_j[j] = suma_p_j[j] + p_ij[i, j];
                }
            }
            int[,] secuencia_operaciones_j = new int[beamWidth, n * m];
            int[,] secuencia_operaciones_i = new int[beamWidth, n * m];
            int[] pi_ini_job = new int[n * m];
            int[] pi_ini_mach = new int[n * m];
            int[] FO_inicial = new int[n];
            int menorFO_final = 0;
            bool PV_final = true;
            for (int j = 0; j < n; j++)
            {
                pi_ini_job[0] = j;
                pi_ini_mach[0] = Ruta_ij[0, j];
                int num_trabajos_en_secuencia = 1;
                BS_FTP1_nondelay(n, m, p_ij, Ruta_ij, num_trabajos_en_secuencia, pi_ini_job, pi_ini_mach, out pi_ini_job, out pi_ini_mach);
                FO_inicial[j] = Decoding_ObR_JS_makespan_v1(m, n, p_ij, pi_ini_job, pi_ini_mach, Ruta_ij);
                if (PV_final == true)
                {
                    PV_final = false;
                    menorFO_final = FO_inicial[j];
                }
                else if (FO_inicial[j] < menorFO_final)
                {
                    menorFO_final = FO_inicial[j];
                }
            }
            int[] orden = new int[n];
            double[,] Elements = new double[2, n];
            for (int k = 0; k < n; k++)
            {
                Elements[0, k] = k;
                Elements[1, k] = FO_inicial[k];
            }
            double[,] ElementsSalida = Quicksort(Elements, 0, n - 1);
            for (int k = 0; k < n; k++)
            {
                orden[k] = (int)ElementsSalida[0, k];
            }
            for (int b = 0; b < beamWidth; b++)
            {
                secuencia_operaciones_j[b, 0] = orden[b];
                secuencia_operaciones_i[b, 0] = Ruta_ij[0, orden[b]];
            }
            for (int it = 1; it < n * m; it++)
            {
                for (int b = 0; b < beamWidth; b++)
                {
                    int[] trabajosFiltrados;
                    int maqFiltrada = Filtro_NonDelay_Schedule(m, n, p_ij, Ruta_ij, filterWidth, it, b, secuencia_operaciones_j, secuencia_operaciones_i, suma_p_j, out trabajosFiltrados);
                    int numTrabFiltrados = trabajosFiltrados.Length;
                    int[] FO_aux = new int[numTrabFiltrados];
                    int menorFO = 0;
                    bool PV = true;
                    int trab_eleg = 0;
                    int maq_eleg = 0;
                    for (int j = 0; j < numTrabFiltrados; j++)
                    {
                        int[] pi_maquina = new int[it + 1];
                        int[] pi_trabajo = new int[it + 1];
                        for (int jj = 0; jj < it; jj++)
                        {
                            pi_maquina[jj] = secuencia_operaciones_i[b, jj];
                            pi_trabajo[jj] = secuencia_operaciones_j[b, jj];
                        }
                        pi_maquina[it] = maqFiltrada;
                        pi_trabajo[it] = trabajosFiltrados[j];
                        int[] pi_job, pi_mach;
                        BS_FTP1_nondelay(n, m, p_ij, Ruta_ij, it + 1, pi_trabajo, pi_maquina, out pi_job, out pi_mach);
                        FO_aux[j] = Decoding_ObR_JS_makespan_v1(m, n, p_ij, pi_job, pi_mach, Ruta_ij);
                        if (PV == true)
                        {
                            PV = false;
                            menorFO = FO_aux[j];
                            trab_eleg = trabajosFiltrados[j];
                            maq_eleg = maqFiltrada;
                        }
                        else if (FO_aux[j] < menorFO)
                        {
                            menorFO = FO_aux[j];
                            trab_eleg = trabajosFiltrados[j];
                            maq_eleg = maqFiltrada;
                        }
                        if (FO_aux[j] < menorFO_final)
                        {
                            menorFO_final = FO_aux[j];
                        }
                    }
                    secuencia_operaciones_j[b, it] = trab_eleg;
                    secuencia_operaciones_i[b, it] = maq_eleg;
                }
            }
            int[] FO_final = new int[beamWidth];
            for (int b = 0; b < beamWidth; b++)
            {
                int[] pi_maquina = new int[n * m];
                int[] pi_trabajo = new int[n * m];
                for (int jj = 0; jj < n * m; jj++)
                {
                    pi_maquina[jj] = secuencia_operaciones_i[b, jj];
                    pi_trabajo[jj] = secuencia_operaciones_j[b, jj];
                }
                FO_final[b] = Decoding_ObR_JS_makespan_v1(m, n, p_ij, pi_trabajo, pi_maquina, Ruta_ij);
                if (FO_final[b] < menorFO_final)
                {
                    menorFO_final = FO_final[b];
                }
            }
            return menorFO_final;
        }
        private static void BS_FTP1_nondelay(int n, int m, int[,] p_ij, int[,] Ruta_ij, int num_elem_actual, int[] sec_op_j, int[] sec_op_i, out int[] pi_trabajo_out, out int[] pi_maquina_out)
        {
            pi_trabajo_out = new int[m * n];
            pi_maquina_out = new int[m * n];
            int[] resto_pj = new int[n];
            for (int j = 0; j < n; j++)
            {
                resto_pj[j] = 0;
                for (int i = 0; i < m; i++)
                {
                    int maq = Ruta_ij[i, j];
                    resto_pj[j] += (i + 1) * p_ij[maq, j];
                }
            }
            int[] elementos_j = new int[n];
            int[] elementos_i = new int[m];
            int[] carga_maq = new int[m];
            int[] carga_trab = new int[n];
            for (int j = 0; j < num_elem_actual; j++)
            {
                int maq = sec_op_i[j];
                int trab = sec_op_j[j];
                resto_pj[trab] = resto_pj[trab] - (elementos_j[trab] + 1) * p_ij[maq, trab];
                elementos_j[trab]++;
                elementos_i[maq]++;
                int comienzo = carga_maq[maq];
                if (carga_trab[trab] > comienzo)
                {
                    comienzo = carga_trab[trab];
                }
                carga_trab[trab] = comienzo + p_ij[maq, trab];
                carga_maq[maq] = comienzo + p_ij[maq, trab];
                pi_trabajo_out[j] = sec_op_j[j];
                pi_maquina_out[j] = sec_op_i[j];
            }
            for (int k = num_elem_actual; k < n * m; k++)
            {
                int comienzo_min = 0;
                int maq_elegida = 0;
                int trabajo_elegido = 0;
                int resto_max = 0;
                bool PV = true;
                for (int j = 0; j < n; j++)
                {
                    if (elementos_j[j] < m)
                    {
                        int maq = Ruta_ij[elementos_j[j], j];
                        int trab = j;
                        int comienzo = carga_maq[maq];
                        if (carga_trab[trab] > comienzo)
                        {
                            comienzo = carga_trab[trab];
                        }
                        if (PV == true)
                        {
                            PV = false;
                            comienzo_min = comienzo;
                            maq_elegida = maq;
                            trabajo_elegido = trab;
                            resto_max = resto_pj[trab];
                        }
                        else if (comienzo < comienzo_min)
                        {
                            comienzo_min = comienzo;
                            maq_elegida = maq;
                            trabajo_elegido = trab;
                            resto_max = resto_pj[trab];
                        }
                        else if (comienzo == comienzo_min && resto_pj[trab] > resto_max)
                        {
                            maq_elegida = maq;
                            trabajo_elegido = trab;
                            resto_max = resto_pj[trab];
                        }
                    }
                }
                resto_pj[trabajo_elegido] = resto_pj[trabajo_elegido] - (elementos_j[trabajo_elegido] + 1) * p_ij[maq_elegida, trabajo_elegido];
                carga_trab[trabajo_elegido] = comienzo_min + p_ij[maq_elegida, trabajo_elegido];
                carga_maq[maq_elegida] = comienzo_min + p_ij[maq_elegida, trabajo_elegido];
                pi_trabajo_out[k] = trabajo_elegido;
                pi_maquina_out[k] = maq_elegida;
                elementos_i[maq_elegida]++;
                elementos_j[trabajo_elegido]++;
            }
        }
        //***BS_FTP1_LS
        private static int BS_FTP1_LS(claseDatos datos, int beamWidth, int filterWidth)
        {
            int m = datos.M;
            int n = datos.N;
            int[,] p_ij = datos.tiemposProceso_ij;
            int[,] Ruta_ij = datos.Ruta_ij;
            claseSolucion solucion = new claseSolucion();
            int[] suma_p_j = new int[n];
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < m; i++)
                {
                    suma_p_j[j] = suma_p_j[j] + p_ij[i, j];
                }
            }            
            int[,] secuencia_operaciones_j = new int[beamWidth, n * m];
            int[,] secuencia_operaciones_i = new int[beamWidth, n * m];
            int[] pi_ini_job = new int[n * m];
            int[] pi_ini_mach = new int[n * m];
            int[] FO_inicial = new int[n];
            int menorFO_final = 0;
            bool PV_final = true;
            for (int j = 0; j < n; j++)
            {
                pi_ini_job[0] = j;
                pi_ini_mach[0] = Ruta_ij[0, j];
                int num_trabajos_en_secuencia = 1;
                BS_FTP1_nondelay(n, m, p_ij, Ruta_ij, num_trabajos_en_secuencia, pi_ini_job, pi_ini_mach, out pi_ini_job, out pi_ini_mach);
                FO_inicial[j] = Decoding_ObR_JS_makespan_v1(m, n, p_ij, pi_ini_job, pi_ini_mach, Ruta_ij);
                if (PV_final == true)
                {
                    PV_final = false;
                    menorFO_final = FO_inicial[j];
                }
                else if (FO_inicial[j] < menorFO_final)
                {
                    menorFO_final = FO_inicial[j];
                }
            }
            int[] orden = new int[n];
            double[,] Elements = new double[2, n];
            for (int k = 0; k < n; k++)
            {
                Elements[0, k] = k;
                Elements[1, k] = FO_inicial[k];
            }
            double[,] ElementsSalida = Quicksort(Elements, 0, n - 1);
            for (int k = 0; k < n; k++)
            {
                orden[k] = (int)ElementsSalida[0, k];
            }
            for (int b = 0; b < beamWidth; b++)
            {
                secuencia_operaciones_j[b, 0] = orden[b];
                secuencia_operaciones_i[b, 0] = Ruta_ij[0, orden[b]];
            }
            for (int it = 1; it < n * m; it++)
            {
                for (int b = 0; b < beamWidth; b++)
                {
                    int[] trabajosFiltrados;
                    int maqFiltrada = Filtro_NonDelay_Schedule(m, n, p_ij, Ruta_ij, filterWidth, it, b, secuencia_operaciones_j, secuencia_operaciones_i, suma_p_j, out trabajosFiltrados);
                    int numTrabFiltrados = trabajosFiltrados.Length;
                    int[] FO_aux = new int[numTrabFiltrados];
                    int menorFO = 0;
                    bool PV = true;
                    int trab_eleg = 0;
                    int maq_eleg = 0;
                    for (int j = 0; j < numTrabFiltrados; j++)
                    {
                        int[] pi_maquina = new int[it + 1];
                        int[] pi_trabajo = new int[it + 1];
                        for (int jj = 0; jj < it; jj++)
                        {
                            pi_maquina[jj] = secuencia_operaciones_i[b, jj];
                            pi_trabajo[jj] = secuencia_operaciones_j[b, jj];
                        }
                        pi_maquina[it] = maqFiltrada;
                        pi_trabajo[it] = trabajosFiltrados[j];
                        int[] pi_job, pi_mach;
                        BS_FTP1_nondelay(n, m, p_ij, Ruta_ij, it + 1, pi_trabajo, pi_maquina, out pi_job, out pi_mach);
                        FO_aux[j] = Decoding_ObR_JS_makespan_v1(m, n, p_ij, pi_job, pi_mach, Ruta_ij);
                        if (PV == true)
                        {
                            PV = false;
                            menorFO = FO_aux[j];
                            trab_eleg = trabajosFiltrados[j];
                            maq_eleg = maqFiltrada;
                        }
                        else if (FO_aux[j] < menorFO)
                        {
                            menorFO = FO_aux[j];
                            trab_eleg = trabajosFiltrados[j];
                            maq_eleg = maqFiltrada;
                        }
                        if (FO_aux[j] < menorFO_final)
                        {
                            menorFO_final = FO_aux[j];
                        }
                    }
                    secuencia_operaciones_j[b, it] = trab_eleg;
                    secuencia_operaciones_i[b, it] = maq_eleg;
                }
            }
            int[] FO_final = new int[beamWidth];
            for (int b = 0; b < beamWidth; b++)
            {
                int[] pi_maquina = new int[n * m];
                int[] pi_trabajo = new int[n * m];
                for (int jj = 0; jj < n * m; jj++)
                {
                    pi_maquina[jj] = secuencia_operaciones_i[b, jj];
                    pi_trabajo[jj] = secuencia_operaciones_j[b, jj];
                }
                FO_final[b] = Decoding_ObR_JS_makespan_v1(m, n, p_ij, pi_trabajo, pi_maquina, Ruta_ij);
                int[,] pi_ij_generica = new int[m, n];
                int[] elementos_i = new int[m];
                for (int jj = 0; jj < n * m; jj++)
                {
                    int trab = pi_trabajo[jj];
                    int maq = pi_maquina[jj];
                    pi_ij_generica[maq, elementos_i[maq]] = trab;
                    elementos_i[maq]++;
                }
                int FOaux = LSi_aceleraciones(m, n, p_ij, pi_ij_generica, Ruta_ij);
                if (FOaux < menorFO_final)
                {
                    menorFO_final = FOaux;
                }
            }
            return menorFO_final;
        }
        //***FTP1
        private static void FTP1(int n, int m, int[,] p_ij, int[,] Ruta_ij, out int[] pi_trabajo_out, out int[] pi_maquina_out)
        {
            pi_trabajo_out = new int[m * n];
            pi_maquina_out = new int[m * n];
            int[] resto_pj = new int[n];
            for (int j = 0; j < n; j++)
            {
                resto_pj[j] = 0;
                for (int i = 0; i < m; i++)
                {
                    int maq = Ruta_ij[i, j];
                    resto_pj[j] += (i + 1) * p_ij[maq, j];
                }
            }
            int[] elementos_j = new int[n];
            int[] elementos_i = new int[m];
            int[] carga_maq = new int[m];
            int[] carga_trab = new int[n];
            for (int k = 0; k < n * m; k++)
            {
                int comienzo_min = 0;
                int maq_elegida = 0;
                int trabajo_elegido = 0;
                int resto_max = 0;
                bool PV = true;
                for (int j = 0; j < n; j++)
                {
                    if (elementos_j[j] < m)
                    {
                        int maq = Ruta_ij[elementos_j[j], j];
                        int trab = j;
                        int comienzo = carga_maq[maq];
                        if (carga_trab[trab] > comienzo)
                        {
                            comienzo = carga_trab[trab];
                        }
                        if (PV == true)
                        {
                            PV = false;
                            comienzo_min = comienzo;
                            maq_elegida = maq;
                            trabajo_elegido = trab;
                            resto_max = resto_pj[trab];
                        }
                        else if (comienzo < comienzo_min)
                        {
                            comienzo_min = comienzo;
                            maq_elegida = maq;
                            trabajo_elegido = trab;
                            resto_max = resto_pj[trab];
                        }
                        else if (comienzo == comienzo_min && resto_pj[trab] > resto_max)
                        {
                            maq_elegida = maq;
                            trabajo_elegido = trab;
                            resto_max = resto_pj[trab];
                        }
                    }
                }
                resto_pj[trabajo_elegido] = resto_pj[trabajo_elegido] - (elementos_j[trabajo_elegido] + 1) * p_ij[maq_elegida, trabajo_elegido];
                carga_trab[trabajo_elegido] = comienzo_min + p_ij[maq_elegida, trabajo_elegido];
                carga_maq[maq_elegida] = comienzo_min + p_ij[maq_elegida, trabajo_elegido];
                pi_trabajo_out[k] = trabajo_elegido;
                pi_maquina_out[k] = maq_elegida;
                elementos_i[maq_elegida]++;
                elementos_j[trabajo_elegido]++;
            }
        }
        //***FTP1_LS
        private static claseSolucion FTP1_LS(claseDatos datos)
        {
            int m = datos.M;
            int n = datos.N;
            int[,] p_ij = datos.tiemposProceso_ij;
            int[,] Ruta_ij = datos.Ruta_ij;
            int[] pi_trabajo_out = new int[m * n];
            int[] pi_maquina_out = new int[m * n];
            int[] resto_pj = new int[n];
            int[,] pi_ij_generica = new int[m, n];
            int[] contador_i = new int[m];
            for (int j = 0; j < n; j++)
            {
                resto_pj[j] = 0;
                for (int i = 0; i < m; i++)
                {
                    int maq = Ruta_ij[i, j];
                    resto_pj[j] += (i + 1) * p_ij[maq, j];
                }
            }
            int[] elementos_j = new int[n];
            int[] elementos_i = new int[m];
            int[] carga_maq = new int[m];
            int[] carga_trab = new int[n];
            for (int k = 0; k < n * m; k++)
            {
                int comienzo_min = 0;
                int maq_elegida = 0;
                int trabajo_elegido = 0;
                int resto_max = 0;
                bool PV = true;
                for (int j = 0; j < n; j++)
                {
                    if (elementos_j[j] < m)
                    {
                        int maq = Ruta_ij[elementos_j[j], j];
                        int trab = j;
                        int comienzo = carga_maq[maq];
                        if (carga_trab[trab] > comienzo)
                        {
                            comienzo = carga_trab[trab];
                        }
                        if (PV == true)
                        {
                            PV = false;
                            comienzo_min = comienzo;
                            maq_elegida = maq;
                            trabajo_elegido = trab;
                            resto_max = resto_pj[trab];
                        }
                        else if (comienzo < comienzo_min)
                        {
                            comienzo_min = comienzo;
                            maq_elegida = maq;
                            trabajo_elegido = trab;
                            resto_max = resto_pj[trab];
                        }
                        else if (comienzo == comienzo_min && resto_pj[trab] > resto_max)
                        {
                            maq_elegida = maq;
                            trabajo_elegido = trab;
                            resto_max = resto_pj[trab];
                        }
                    }
                }
                resto_pj[trabajo_elegido] = resto_pj[trabajo_elegido] - (elementos_j[trabajo_elegido] + 1) * p_ij[maq_elegida, trabajo_elegido];
                carga_trab[trabajo_elegido] = comienzo_min + p_ij[maq_elegida, trabajo_elegido];
                carga_maq[maq_elegida] = comienzo_min + p_ij[maq_elegida, trabajo_elegido];
                pi_trabajo_out[k] = trabajo_elegido;
                pi_maquina_out[k] = maq_elegida;
                pi_ij_generica[maq_elegida, elementos_i[maq_elegida]] = trabajo_elegido;
                elementos_i[maq_elegida]++;
                elementos_j[trabajo_elegido]++;
            }
            int mejorFO = 0;
            int[,] secuenciaAuxiliar = new int[m, n];
            claseSolucion solucion = new claseSolucion();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    secuenciaAuxiliar[i, j] = pi_ij_generica[i, j];
                }
            }
            mejorFO = LSi_aceleraciones(m, n, p_ij, pi_ij_generica, Ruta_ij);
            solucion.makespan = mejorFO;
            solucion.secuenciaSolucionExtendida = new int[m, n];
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < m; i++)
                {
                    solucion.secuenciaSolucionExtendida[i, j] = pi_ij_generica[i, j];
                }
            }
            int makespan1 = solucion.makespan;
            return solucion;
        }
        //***FTP2
        private static claseSolucion FTP2(claseDatos datos, int x)
        {
            int m = datos.M;
            int n = datos.N;
            int[,] p_ij = datos.tiemposProceso_ij;
            int[,] Ruta_ij = datos.Ruta_ij;
            int[] pi_trabajo_out = new int[m * n];
            int[] pi_maquina_out = new int[m * n];
            int[] resto_pj = new int[n];
            for (int j = 0; j < n; j++)
            {
                resto_pj[j] = 0;
                for (int i = 0; i < m; i++)
                {
                    int maq = Ruta_ij[i, j];
                    resto_pj[j] += (i + 1) * p_ij[maq, j];
                }
            }
            int[,] pi_ij_generica = new int[m, n];
            int[,] mejor_pi_ij = new int[m, n];
            int mejorFO = 0;
            int[] alpha = new int[n];
            double[,] Elements = new double[2, n];
            for (int k = 0; k < n; k++)
            {
                Elements[0, k] = k;
                Elements[1, k] = p_ij[Ruta_ij[0, k], k];
            }
            double[,] ElementsSalida = Quicksort_decre(Elements, 0, n - 1);
            for (int k = 0; k < n; k++)
            {
                alpha[k] = (int)ElementsSalida[0, k];
            }
            for (int i_x = 0; i_x < x; i_x++)
            {
                int[] elementos_j = new int[n];
                int[] elementos_i = new int[m];
                int[] carga_maq = new int[m];
                int[] carga_trab = new int[n];
                for (int j = 0; j < n; j++)
                {
                    resto_pj[j] = 0;
                    for (int i = 0; i < m; i++)
                    {
                        int maq1 = Ruta_ij[i, j];
                        resto_pj[j] += (i + 1) * p_ij[maq1, j];
                    }
                }
                int maq = Ruta_ij[0, alpha[i_x]];
                int trab = alpha[i_x];
                int comienzo = carga_maq[maq];
                if (carga_trab[trab] > comienzo)
                {
                    comienzo = carga_trab[trab];
                }
                resto_pj[trab] = resto_pj[trab] - (elementos_j[trab] + 1) * p_ij[maq, trab];
                carga_trab[trab] = comienzo + p_ij[maq, trab];
                carga_maq[maq] = comienzo + p_ij[maq, trab];
                pi_trabajo_out[0] = trab;
                pi_maquina_out[0] = maq;
                pi_ij_generica[maq, elementos_i[maq]] = trab;
                elementos_i[maq]++;
                elementos_j[trab]++;
                for (int k = 1; k < n * m; k++)
                {
                    int comienzo_min = 0;
                    int maq_elegida = 0;
                    int trabajo_elegido = 0;
                    int resto_max = 0;
                    bool PV = true;
                    for (int j = 0; j < n; j++)
                    {
                        if (elementos_j[j] < m)
                        {
                            maq = Ruta_ij[elementos_j[j], j];
                            trab = j;
                            comienzo = carga_maq[maq];
                            if (carga_trab[trab] > comienzo)
                            {
                                comienzo = carga_trab[trab];
                            }
                            if (PV == true)
                            {
                                PV = false;
                                comienzo_min = comienzo;
                                maq_elegida = maq;
                                trabajo_elegido = trab;
                                resto_max = resto_pj[trab];
                            }
                            else if (comienzo < comienzo_min)
                            {
                                comienzo_min = comienzo;
                                maq_elegida = maq;
                                trabajo_elegido = trab;
                                resto_max = resto_pj[trab];
                            }
                            else if (comienzo == comienzo_min && resto_pj[trab] > resto_max)
                            {
                                maq_elegida = maq;
                                trabajo_elegido = trab;
                                resto_max = resto_pj[trab];
                            }
                        }
                    }
                    resto_pj[trabajo_elegido] = resto_pj[trabajo_elegido] - (elementos_j[trabajo_elegido] + 1) * p_ij[maq_elegida, trabajo_elegido];
                    carga_trab[trabajo_elegido] = comienzo_min + p_ij[maq_elegida, trabajo_elegido];
                    carga_maq[maq_elegida] = comienzo_min + p_ij[maq_elegida, trabajo_elegido];
                    pi_trabajo_out[k] = trabajo_elegido;
                    pi_maquina_out[k] = maq_elegida;
                    pi_ij_generica[maq_elegida, elementos_i[maq_elegida]] = trabajo_elegido;
                    elementos_i[maq_elegida]++;
                    elementos_j[trabajo_elegido]++;
                }
                int solucionFO = Cmax_SecPorMaq(m, n, p_ij, pi_ij_generica, Ruta_ij);
                int FOaux = LSi_aceleraciones(m, n, p_ij, pi_ij_generica, Ruta_ij);
                if (i_x == 0 || FOaux != -1 && FOaux < mejorFO)
                {
                    mejorFO = FOaux;

                    for (int i = 0; i < m; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            mejor_pi_ij[i, j] = pi_ij_generica[i, j];
                        }
                    }
                }
            }
            int[,] secuenciaAuxiliar = new int[m, n];
            claseSolucion solucion = new claseSolucion();
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    secuenciaAuxiliar[i, j] = mejor_pi_ij[i, j];
                }
            }
            solucion.makespan = mejorFO;
            solucion.secuenciaSolucionExtendida = new int[m, n];
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < m; i++)
                {
                    solucion.secuenciaSolucionExtendida[i, j] = mejor_pi_ij[i, j];
                }
            }
            int makespan1 = solucion.makespan;
            return solucion;
        }
        private static int Cmax_SecPorMaq(int m, int n, int[,] p_ij, int[,] solucion, int[,] Ruta)
        {
            int[] elementoActual_j = new int[m];
            int[] elementoActual_i = new int[n];
            int[,] r_j = new int[m, n];
            int[] carga_j = new int[n];
            int[] carga_i = new int[m];
            bool[] terminado_i = new bool[m];
            for (int i = 0; i < m; i++)
            {
                terminado_i[i] = false;
            }
            int terminados = 0;
            int Cmax = 0;
            while (terminados < m && Cmax != -1)
            {
                bool elegido = false;
                int i = 0;
                for (i = 0; i < m; i++)
                {
                    if (terminado_i[i] == false)
                    {
                        int trabajo = solucion[i, elementoActual_j[i]];
                        if (Ruta[elementoActual_i[trabajo], trabajo] == i)
                        {
                            r_j[i, trabajo] = Math.Max(carga_j[trabajo], carga_i[i]);
                            carga_j[trabajo] = r_j[i, trabajo] + p_ij[i, trabajo];
                            carga_i[i] = r_j[i, trabajo] + p_ij[i, trabajo];
                            elementoActual_j[i]++;
                            elementoActual_i[trabajo]++;
                            if (elementoActual_j[i] >= n)
                            {
                                terminados++;
                                terminado_i[i] = true;
                            }
                            elegido = true;
                        }
                    }
                }
                if (elegido == false)
                {
                    Cmax = -1;
                }
            }
            if (Cmax != -1)
            {
                Cmax = carga_i[0];
                for (int i = 0; i < m; i++)
                {
                    if (carga_i[i] > Cmax)
                    {
                        Cmax = carga_i[i];
                    }
                }
            }
            return Cmax;
        }
        private static int LSi_aceleraciones(int m, int n, int[,] p_ij, int[,] sec, int[,] Ruta_ij)
        {
            int Cmax = -1;
            bool improve = true;
            while (improve == true)
            {
                improve = false;
                int[,] C_ij_forward;
                int[,] C_ij_backward;
                int[,] operacionesCC;
                int contadorOperCC;
                Cmax = Cmax_Forward(m, n, p_ij, sec, Ruta_ij, out C_ij_forward);
                Cmax_Backward(m, n, p_ij, sec, Ruta_ij, Cmax, C_ij_forward, out C_ij_backward, out operacionesCC, out contadorOperCC);
                for (int j = 0; j < contadorOperCC; j++)
                {
                    int posMaq = operacionesCC[j, 0];
                    int maq = operacionesCC[j, 1];
                    int posJob = operacionesCC[j, 2];
                    int job = operacionesCC[j, 3];
                    int[] sec_maq = new int[n];
                    for (int j_aux2 = 0; j_aux2 < n - 1; j_aux2++)
                    {
                        if (j_aux2 >= posJob)
                        {
                            sec_maq[j_aux2] = sec[maq, j_aux2 + 1];
                        }
                        else
                        {
                            sec_maq[j_aux2] = sec[maq, j_aux2];
                        }
                    }
                    int Cmax_red = Cmax_Forward_operMis(m, n, p_ij, sec, Ruta_ij, sec_maq, maq, job, out C_ij_forward);
                    Cmax_Backward_operMis(m, n, p_ij, sec, Ruta_ij, Cmax_red, sec_maq, maq, job, out C_ij_backward);
                    int minimoFOLocal = 0;
                    int mejorJJ = 0;
                    bool primeraVez = true;
                    for (int jj = 0; jj < n; jj++)
                    {
                        int Forward = 0;
                        if (jj > 0)
                        {
                            Forward = C_ij_forward[maq, sec_maq[jj - 1]];
                        }
                        if (posMaq > 0)
                        {
                            Forward = Math.Max(C_ij_forward[Ruta_ij[posMaq - 1, job], job], Forward);
                        }
                        int Backward = 0;
                        if (jj < n - 1)
                        {
                            Backward = Cmax_red - C_ij_backward[maq, sec_maq[jj]] + p_ij[maq, sec_maq[jj]];
                        }
                        if (posMaq < m - 1)
                        {
                            Backward = Math.Max(Cmax_red - C_ij_backward[Ruta_ij[posMaq + 1, job], job] + p_ij[Ruta_ij[posMaq + 1, job], job], Backward);
                        }
                        int new_Cmax = Forward + p_ij[maq, job] + Backward;
                        new_Cmax = Math.Max(new_Cmax, Cmax_red);
                        if (primeraVez == true || new_Cmax < minimoFOLocal)
                        {
                            primeraVez = false;
                            minimoFOLocal = new_Cmax;
                            mejorJJ = jj;
                        }
                    }
                    if (minimoFOLocal < Cmax)
                    {
                        int[] sec_maq_aux = new int[n];
                        for (int j_aux2 = 0; j_aux2 < n; j_aux2++)
                        {
                            if (j_aux2 == mejorJJ)
                            {
                                sec_maq_aux[j_aux2] = job;
                            }
                            else if (j_aux2 < mejorJJ)
                            {
                                sec_maq_aux[j_aux2] = sec_maq[j_aux2];
                            }
                            else if (j_aux2 > mejorJJ)
                            {
                                sec_maq_aux[j_aux2] = sec_maq[j_aux2 - 1];
                            }
                        }
                        int[,] C_ij_forward_2;
                        int[,] C_ij_backward_2;
                        int CmaxFW2 = Cmax_Forward(m, n, p_ij, sec, Ruta_ij, sec_maq_aux, maq, out C_ij_forward_2);
                        bool factible = true;
                        if (CmaxFW2 == -1)
                        {
                            factible = false;
                        }
                        if (factible == true)
                        {
                            for (int j_aux2 = 0; j_aux2 < n; j_aux2++)
                            {
                                if (j_aux2 == mejorJJ)
                                {
                                    sec[maq, j_aux2] = job;
                                }
                                else if (j_aux2 < mejorJJ)
                                {
                                    sec[maq, j_aux2] = sec_maq[j_aux2];
                                }
                                else if (j_aux2 > mejorJJ)
                                {
                                    sec[maq, j_aux2] = sec_maq[j_aux2 - 1];
                                }
                            }
                            Cmax_Backward(m, n, p_ij, sec, Ruta_ij, CmaxFW2, C_ij_forward_2, out C_ij_backward_2, out operacionesCC, out contadorOperCC);
                            Cmax = minimoFOLocal;
                            improve = true;
                        }
                    }
                }
            }
            return Cmax;
        }
        private static int Cmax_Forward(int m, int n, int[,] p_ij, int[,] solucion, int[,] Ruta, int[] sec_maq, int maq, out int[,] C_ij)
        {
            int[] elementoActual_j = new int[m];
            int[] elementoActual_i = new int[n];
            C_ij = new int[m, n];
            int[] carga_j = new int[n];
            int[] carga_i = new int[m];
            bool[] terminado_j = new bool[n];
            for (int j = 0; j < n; j++)
            {
                terminado_j[j] = false;
            }
            int terminados = 0;
            int Cmax = 0;
            while (terminados < n && Cmax != -1)
            {
                bool elegido = false;
                int j = 0;
                for (j = 0; j < n; j++)
                {
                    if (terminado_j[j] == false)
                    {
                        int machina_oper = Ruta[elementoActual_i[j], j];
                        if (machina_oper != maq)
                        {
                            if (solucion[machina_oper, elementoActual_j[machina_oper]] == j)
                            {
                                C_ij[machina_oper, j] = Math.Max(carga_j[j], carga_i[machina_oper]) + p_ij[machina_oper, j];
                                carga_j[j] = C_ij[machina_oper, j];
                                carga_i[machina_oper] = C_ij[machina_oper, j];
                                elementoActual_j[machina_oper]++;
                                elementoActual_i[j]++;
                                if (elementoActual_i[j] >= m)
                                {
                                    terminados++;
                                    terminado_j[j] = true;
                                }
                                elegido = true;
                            }
                        }
                        else
                        {
                            if (sec_maq[elementoActual_j[machina_oper]] == j)
                            {
                                C_ij[machina_oper, j] = Math.Max(carga_j[j], carga_i[machina_oper]) + p_ij[machina_oper, j];
                                carga_j[j] = C_ij[machina_oper, j];
                                carga_i[machina_oper] = C_ij[machina_oper, j];
                                elementoActual_j[machina_oper]++;
                                elementoActual_i[j]++;
                                if (elementoActual_i[j] >= m)
                                {
                                    terminados++;
                                    terminado_j[j] = true;
                                }
                                elegido = true;
                            }
                        }
                    }
                }
                if (elegido == false)
                {
                    Cmax = -1;
                }
            }
            if (Cmax != -1)
            {
                Cmax = carga_i[0];
                for (int i = 0; i < m; i++)
                {
                    if (carga_i[i] > Cmax)
                    {
                        Cmax = carga_i[i];
                    }
                }
            }
            return Cmax;
        }
        private static int Cmax_Forward(int m, int n, int[,] p_ij, int[,] solucion, int[,] Ruta, out int[,] C_ij)
        {
            int[] elementoActual_j = new int[m];
            int[] elementoActual_i = new int[n];
            C_ij = new int[m, n];
            int[] carga_j = new int[n];
            int[] carga_i = new int[m];
            bool[] terminado_j = new bool[n];
            for (int j = 0; j < n; j++)
            {
                terminado_j[j] = false;
            }
            int terminados = 0;
            int Cmax = 0;
            while (terminados < n && Cmax != -1)
            {
                bool elegido = false;
                int j = 0;
                for (j = 0; j < n; j++)
                {
                    if (terminado_j[j] == false)
                    {
                        int machina_oper = Ruta[elementoActual_i[j], j];
                        if (solucion[machina_oper, elementoActual_j[machina_oper]] == j)
                        {
                            C_ij[machina_oper, j] = Math.Max(carga_j[j], carga_i[machina_oper]) + p_ij[machina_oper, j];
                            carga_j[j] = C_ij[machina_oper, j];
                            carga_i[machina_oper] = C_ij[machina_oper, j];
                            elementoActual_j[machina_oper]++;
                            elementoActual_i[j]++;
                            if (elementoActual_i[j] >= m)
                            {
                                terminados++;
                                terminado_j[j] = true;
                            }
                            elegido = true;
                        }
                    }
                }
                if (elegido == false)
                {
                    Cmax = -1;
                }
            }
            if (Cmax != -1)
            {
                Cmax = carga_i[0];
                for (int i = 0; i < m; i++)
                {
                    if (carga_i[i] > Cmax)
                    {
                        Cmax = carga_i[i];
                    }
                }
            }
            return Cmax;
        }
        private static void Cmax_Backward(int m, int n, int[,] p_ij, int[,] solucion, int[,] Ruta, int Cmax, int[,] C_ij_FW, out int[,] C_ij_BW, out int[,] operacionEnCaminoCritico, out int contadorOperCC)
        {
            int[] elementoActual_j = new int[m];
            int[] elementoActual_i = new int[n];
            C_ij_BW = new int[m, n];
            operacionEnCaminoCritico = new int[n * m, 4];//PosMaq,Maq,PosJob,Job
            contadorOperCC = 0;
            int[] carga_j = new int[n];
            int[] carga_i = new int[m];
            bool[] terminado_j = new bool[n];
            for (int j = 0; j < n; j++)
            {
                terminado_j[j] = false;
                carga_j[j] = Cmax;
                elementoActual_i[j] = m - 1;
            }
            for (int i = 0; i < m; i++)
            {
                carga_i[i] = Cmax;
                elementoActual_j[i] = n - 1;
            }
            int terminados = 0;
            while (terminados < n)
            {
                for (int j = 0; j < n; j++)
                {
                    if (terminado_j[j] == false)
                    {
                        int machina_oper = Ruta[elementoActual_i[j], j];
                        if (solucion[machina_oper, elementoActual_j[machina_oper]] == j)
                        {
                            C_ij_BW[machina_oper, j] = Math.Min(carga_j[j], carga_i[machina_oper]);
                            carga_j[j] = C_ij_BW[machina_oper, j] - p_ij[machina_oper, j];
                            carga_i[machina_oper] = C_ij_BW[machina_oper, j] - p_ij[machina_oper, j];
                            elementoActual_j[machina_oper]--;
                            elementoActual_i[j]--;
                            if (elementoActual_i[j] < 0)
                            {
                                terminados++;
                                terminado_j[j] = true;
                            }
                        }
                        if (C_ij_BW[machina_oper, j] == C_ij_FW[machina_oper, j])
                        {
                            operacionEnCaminoCritico[contadorOperCC, 0] = elementoActual_i[j] + 1;
                            operacionEnCaminoCritico[contadorOperCC, 1] = machina_oper;
                            operacionEnCaminoCritico[contadorOperCC, 2] = elementoActual_j[machina_oper] + 1;
                            operacionEnCaminoCritico[contadorOperCC, 3] = j;
                            contadorOperCC++;
                        }

                    }
                }
            }
        }
        private static int Cmax_Forward_operMis(int m, int n, int[,] p_ij, int[,] solucion, int[,] Ruta, int[] secMaq, int maqMis, int jobMis, out int[,] C_ij)
        {
            int[] elementoActual_j = new int[m];
            int[] elementoActual_i = new int[n];
            C_ij = new int[m, n];
            int[] carga_j = new int[n];
            int[] carga_i = new int[m];
            bool[] terminado_j = new bool[n];
            for (int j = 0; j < n; j++)
            {
                terminado_j[j] = false;
            }
            int terminados = 0;
            int Cmax = 0;
            if (Ruta[0, jobMis] == maqMis)
            {
                elementoActual_i[jobMis]++;
            }
            while (terminados < n && Cmax != -1)
            {
                bool elegido = false;
                int j = 0;
                for (j = 0; j < n; j++)
                {
                    if (terminado_j[j] == false)
                    {
                        int machina_oper = Ruta[elementoActual_i[j], j];
                        if (machina_oper != maqMis)
                        {
                            if (solucion[machina_oper, elementoActual_j[machina_oper]] == j)
                            {
                                C_ij[machina_oper, j] = Math.Max(carga_j[j], carga_i[machina_oper]) + p_ij[machina_oper, j];
                                carga_j[j] = C_ij[machina_oper, j];
                                carga_i[machina_oper] = C_ij[machina_oper, j];
                                elementoActual_j[machina_oper]++;
                                elementoActual_i[j]++;
                                if (j == jobMis)
                                {
                                    if (elementoActual_i[j] <= m - 1)
                                        if (Ruta[elementoActual_i[j], jobMis] == maqMis)
                                        {
                                            elementoActual_i[j]++;
                                        }
                                    if (elementoActual_i[j] >= m)
                                    {
                                        terminados++;
                                        terminado_j[j] = true;
                                    }
                                }
                                else
                                {
                                    if (elementoActual_i[j] >= m)
                                    {
                                        terminados++;
                                        terminado_j[j] = true;
                                    }
                                }
                                elegido = true;
                            }
                        }
                        else
                        {
                            if (secMaq[elementoActual_j[machina_oper]] == j && j != jobMis)
                            {
                                C_ij[machina_oper, j] = Math.Max(carga_j[j], carga_i[machina_oper]) + p_ij[machina_oper, j];
                                carga_j[j] = C_ij[machina_oper, j];
                                carga_i[machina_oper] = C_ij[machina_oper, j];
                                elementoActual_j[machina_oper]++;
                                elementoActual_i[j]++;
                                if (elementoActual_i[j] >= m)
                                {
                                    terminados++;
                                    terminado_j[j] = true;
                                }
                                elegido = true;
                            }
                        }
                    }
                }
                if (elegido == false)
                {
                    Cmax = -1;
                }
            }
            if (Cmax != -1)
            {
                Cmax = carga_i[0];
                for (int i = 0; i < m; i++)
                {
                    if (carga_i[i] > Cmax)
                    {
                        Cmax = carga_i[i];
                    }
                }
            }
            return Cmax;
        }
        private static void Cmax_Backward_operMis(int m, int n, int[,] p_ij, int[,] solucion, int[,] Ruta, int Cmax, int[] secMaq, int maqMis, int jobMis, out int[,] C_ij_BW)
        {
            int[] elementoActual_j = new int[m];
            int[] elementoActual_i = new int[n];
            C_ij_BW = new int[m, n];
            int[] carga_j = new int[n];
            int[] carga_i = new int[m];
            bool[] terminado_j = new bool[n];
            for (int j = 0; j < n; j++)
            {
                terminado_j[j] = false;
                carga_j[j] = Cmax;
                elementoActual_i[j] = m - 1;
            }
            for (int i = 0; i < m; i++)
            {
                carga_i[i] = Cmax;
                elementoActual_j[i] = n - 1;
                if (i == maqMis)
                {
                    elementoActual_j[i] = n - 2;
                }
            }
            if (Ruta[elementoActual_i[jobMis], jobMis] == maqMis)
            {
                elementoActual_i[jobMis]--;
            }
            int terminados = 0;
            while (terminados < n)
            {
                for (int j = 0; j < n; j++)
                {
                    if (terminado_j[j] == false)
                    {
                        int machina_oper = Ruta[elementoActual_i[j], j];
                        if (machina_oper != maqMis)
                        {
                            if (solucion[machina_oper, elementoActual_j[machina_oper]] == j)
                            {
                                C_ij_BW[machina_oper, j] = Math.Min(carga_j[j], carga_i[machina_oper]);
                                carga_j[j] = C_ij_BW[machina_oper, j] - p_ij[machina_oper, j];
                                carga_i[machina_oper] = C_ij_BW[machina_oper, j] - p_ij[machina_oper, j];
                                elementoActual_j[machina_oper]--;
                                elementoActual_i[j]--;
                                if (j == jobMis)
                                {
                                    if (elementoActual_i[j] >= 0)
                                    {
                                        if (Ruta[elementoActual_i[j], jobMis] == maqMis)
                                        {
                                            elementoActual_i[j]--;
                                        }
                                    }
                                    if (elementoActual_i[j] < 0)
                                    {
                                        terminados++;
                                        terminado_j[j] = true;
                                    }
                                }
                                else
                                {
                                    if (elementoActual_i[j] < 0)
                                    {
                                        terminados++;
                                        terminado_j[j] = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (secMaq[elementoActual_j[machina_oper]] == j && j != jobMis)
                            {
                                C_ij_BW[machina_oper, j] = Math.Min(carga_j[j], carga_i[machina_oper]);
                                carga_j[j] = C_ij_BW[machina_oper, j] - p_ij[machina_oper, j];
                                carga_i[machina_oper] = C_ij_BW[machina_oper, j] - p_ij[machina_oper, j];
                                elementoActual_j[machina_oper]--;
                                elementoActual_i[j]--;
                                if (elementoActual_i[j] < 0)
                                {
                                    terminados++;
                                    terminado_j[j] = true;
                                }
                            }
                        }
                    }
                }
            }
        }
        public static double[,] Quicksort_decre(double[,] elements, int left, int right)
        {
            int i = left, j = right;
            double pivot = elements[1, (left + right) / 2];

            while (i <= j)
            {
                while (elements[1, i].CompareTo(pivot) > 0)
                {
                    i++;
                }

                while (elements[1, j].CompareTo(pivot) < 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    double tmp0 = elements[0, i];
                    double tmp1 = elements[1, i];
                    elements[0, i] = elements[0, j];
                    elements[1, i] = elements[1, j];
                    elements[0, j] = tmp0;
                    elements[1, j] = tmp1;

                    i++;
                    j--;
                }
            }

            if (left < j)
            {
                Quicksort_decre(elements, left, j);
            }

            if (i < right)
            {
                Quicksort_decre(elements, i, right);
            }
            return elements;
        }
        public static double[,] Quicksort(double[,] elements, int left, int right)
        {
            int i = left, j = right;
            double pivot = elements[1, (left + right) / 2];

            while (i <= j)
            {
                while (elements[1, i].CompareTo(pivot) < 0)
                {
                    i++;
                }

                while (elements[1, j].CompareTo(pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    double tmp0 = elements[0, i];
                    double tmp1 = elements[1, i];
                    elements[0, i] = elements[0, j];
                    elements[1, i] = elements[1, j];
                    elements[0, j] = tmp0;
                    elements[1, j] = tmp1;

                    i++;
                    j--;
                }
            }

            if (left < j)
            {
                Quicksort(elements, left, j);
            }

            if (i < right)
            {
                Quicksort(elements, i, right);
            }
            return elements;
        }
    }
}
