using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace TestMySpline
{

    class Program
    {
        public class WaveLengthData
        {
            public string NM { get; set; }
            public string N { get; set; }
            public string K { get; set; }
        }

        public class SiData
        {
            public string NM { get; set; }
            public string N { get; set; }
            public string K { get; set; }
        }

        public class SiNData
        {
            public string NM { get; set; }
            public string N { get; set; }
            public string K { get; set; }
        }

        public class SIO2Data
        {
            public string NM { get; set; }
            public string N { get; set; }
            public string K { get; set; }
        }

        static void Main(string[] args)
        {
            //TestTdm();
            TestSplineOnWikipediaExample();
            //TestSpline();
            //TestPerf();
            //TestFitParametric();

        }

        private static void TestFitParametric()
        {
            // Create the data to be fitted
            float[] x = { 0.5f, 2.0f, 3.0f, 4.5f, 3.0f, 2.0f };
            float[] y = { 4.0f, 2.0f, 6.0f, 4.0f, 3.0f, 5.0f };

            float[] xs, ys;
            CubicSpline.FitParametric(x, y, 100, out xs, out ys);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit", x, y, xs, ys, @"..\..\testSplineFitParametric.png");

            // Specify start slope
            CubicSpline.FitParametric(x, y, 100, out xs, out ys, 1, 0);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Start Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_start_1.png");

            CubicSpline.FitParametric(x, y, 100, out xs, out ys, 0, 1);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Start Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_start_2.png");

            CubicSpline.FitParametric(x, y, 100, out xs, out ys, 1, -1);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Start Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_start_3.png");

            CubicSpline.FitParametric(x, y, 100, out xs, out ys, -1, -1);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Start Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_start_4.png");

            CubicSpline.FitParametric(x, y, 100, out xs, out ys, -1, 1);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Start Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_start_5.png");

            CubicSpline.FitParametric(x, y, 100, out xs, out ys, 1, -1, 1, 1);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Boundary Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_both_1.png");

            CubicSpline.FitParametric(x, y, 100, out xs, out ys, 1, -1, 1, -1);
            PlotSplineSolution("Cubic Spline Interpolation - Parametric Fit - Boundary Slope Specified", x, y, xs, ys, @"..\..\testSplineFitParametric_both_2.png");
        }

        private static void TestSpline()
        {
            int n = 6;

            // Create the data to be fitted
            float[] x = new float[n];
            float[] y = new float[n];
            Random rand = new Random(1);

            for (int i = 0; i < n; i++)
            {
                x[i] = i;
                y[i] = (float)rand.NextDouble() * 10;
            }

            // Compute the x values at which we will evaluate the spline.
            // Upsample the original data by a const factor.
            int upsampleFactor = 10;
            int nInterpolated = n * upsampleFactor;
            float[] xs = new float[nInterpolated];

            for (int i = 0; i < nInterpolated; i++)
            {
                xs[i] = (float)i * (n - 1) / (float)(nInterpolated - 1);
            }

            float[] ys = CubicSpline.Compute(x, y, xs, 0.0f, Single.NaN, true);

            string path = "testSpline.png";
            PlotSplineSolution("Cubic Spline Interpolation - Random Data", x, y, xs, ys, path);
        }

        private static void TestPerf()
        {
            int n = 10000;

            // Create the data to be fitted
            float[] x = new float[n];
            float[] y = new float[n];
            Random rand = new Random(1);

            for (int i = 0; i < n; i++)
            {
                x[i] = i;
                y[i] = (float)rand.NextDouble() * 10;
            }

            // Compute the x values that we will evaluate the spline at.
            // Upsample the original data by a const factor.
            int upsampleFactor = 10;
            int nInterpolate = n * upsampleFactor;
            float[] xs = new float[nInterpolate];

            for (int i = 0; i < nInterpolate; i++)
            {
                xs[i] = (float)i / upsampleFactor;
            }

            // For perf, test multiple reps
            int reps = 100;
            DateTime start = DateTime.Now;

            for (int i = 0; i < reps; i++)
            {
                float[] ys = CubicSpline.Compute(x, y, xs);
            }

            TimeSpan duration = DateTime.Now - start;
            Console.WriteLine("CubicSpline upsample from {0:n0} to {1:n0} points took {2:0.00} ms for {3} iterations ({2:0.000} ms per iteration)",
                n, nInterpolate, duration.TotalMilliseconds, reps, duration.TotalMilliseconds / reps);

            // Compare to NRinC
            //float[] y2 = new float[n];
            //float[] ys2 = new float[nInterpolate];
            //start = DateTime.Now;

            //for (int i = 0; i < reps; i++)
            //{
            //	CubicSplineNR.Spline(x, y, y2);
            //	CubicSplineNR.EvalSpline(x, y, y2, xs, ys2);
            //}

            //duration = DateTime.Now - start;
            //Console.WriteLine("CubicSplineNR upsample from {0:n0} to {1:n0} points took {2:0.00} ms for {3} iterations ({2:0.000} ms per iteration)",
            //	n, nInterpolate, duration.TotalMilliseconds, reps, duration.TotalMilliseconds / reps);
        }

        /// <summary>
        /// This is the Wikipedia "Spline Interpolation" article example.
        /// </summary>
        private static void TestSplineOnWikipediaExample()
        {
            //파일 읽고 -> 클래스에 파일 데이터(nm,n,k)저장 -> stringToSingle -> 계산 -> 그래프 그리기(Plot)
            //파일 입력
            string[] Silines = File.ReadAllLines("C://SiN.txt", Encoding.Default);
            string[] WaveData = File.ReadAllLines("c://SiO2 2nm_on_Si.dat", Encoding.Default);


            List<WaveLengthData> waveLengthDatas = new List<WaveLengthData>();
            List<SiData> ReadData = new List<SiData>();
            List<SiNData> SINDATA = new List<SiNData>();
            List<SIO2Data> sIO2Datas = new List<SIO2Data>();


            //x,y 배열
            float ValX = 0.0f, ValY1 = 0.0f, ValY2 = 0.0f;

            //파일 읽기위해 여백 및 기타 제거
            char[] replace = { ' ', ',', '\t', '\n', Convert.ToChar(10), Convert.ToChar(13) };

            //파일 생성 및 데이터 add
            using (StreamWriter newdatFile = new StreamWriter(new FileStream("SiN_new.txt", FileMode.Create)))
            {

                //// Create the test points.
                //float[] x = new float[] { };
                //float[] yToN = new float[] { };
                //float[] yToK = new float[] { };

                foreach (var line in Silines)
                {
                    string[] splitData = line.Split(replace, StringSplitOptions.RemoveEmptyEntries);
                    if (ReadData.Count <= 1)
                    {
                        ReadData.Add(new SiData());
                    }
                    if (ReadData.Count > 1)
                    {
                        ReadData.Add(
                        new SiData
                        {
                            NM = splitData[0],
                            N = splitData[1],
                            K = splitData[2]
                        });
                    }
                }

                List<float> ListOfFloatNM = new List<float>();
                List<float> ListOfFloatN = new List<float>();
                List<float> ListOfFloatK = new List<float>();
               
                for (int i = 3; i < Silines.Length; i++)
                {
                    ValX = float.Parse(ReadData[i].NM);
                    ValY1 = float.Parse(ReadData[i].N);
                    ValY2 = float.Parse(ReadData[i].K);
                    
                    
                    ListOfFloatNM.Add(ValX);
                    ListOfFloatN.Add(ValY1);
                    ListOfFloatK.Add(ValY2);
                }
                //ListOfFloatNM.RemoveRange(0, 3);
                //ListOfFloatN.RemoveRange(0, 3);
                //ListOfFloatK.RemoveRange(0, 3);

                float[] x = ListOfFloatNM.ToArray();
                float[] yToN = ListOfFloatN.ToArray();
                float[] yToK = ListOfFloatK.ToArray();


                for (int i = 0; i < yToN.Length; i++)
                {       
                    x[i] = (float)Math.Round(x[i], 5);

                    yToN[i] = (float)Math.Round(yToN[i], 5);

                    yToK[i] = (float)Math.Round(yToK[i], 5);

                }





                // silineNum 1470
                // 배열의 길이는 1466

                int n = 2;
                float[] xs = new float[n + 1];
                float stepSize = (x[x.Length-1]-x[0]) / (n - 1);
                for (int i = 0; i < n; i++)
                {
                    xs[i] = x[0] + i + stepSize;
                }

                //fit and eval
                CubicSpline spline = new CubicSpline();
          
                float[] ysToN = spline.FitAndEval(x, yToN, xs);
                float[] ysToK = spline.FitAndEval(x, yToN, xs);

                //plot
                string pathToN = "spline-Si-ToN.png";
                string pathToK = "spline-Si-ToK.png";

                PlotSplineSolution("DolargeSplineToN", x, yToN, xs, ysToN, pathToN);
                PlotSplineSolution("DolargeSplineToK", x, yToK, xs, ysToN, pathToK);


            }


        }



        private static TriDiagonalMatrixF TestTdm()
        {
            TriDiagonalMatrixF m = new TriDiagonalMatrixF(10);

            for (int i = 0; i < m.N; i++)
            {
                m.A[i] = 1.111111f;
                m.B[i] = 2.222222f;
                m.C[i] = 3.333333f;
            }

            Console.WriteLine("Matrix:\n{0}", m.ToDisplayString(",4:0.00", "    "));

            for (int i = 0; i < m.N; i++)
            {
                m[i, i] = 4.4444f;
            }

            Console.WriteLine("Matrix:\n{0}", m.ToDisplayString(",4:0.00", "    "));

            // Solve
            Random rand = new Random(1);
            float[] d = new float[m.N];

            for (int i = 0; i < d.Length; i++)
            {
                d[i] = (float)rand.NextDouble();
            }

            float[] x = m.Solve(d);

            Console.WriteLine("Solve returned: ");

            for (int i = 0; i < x.Length; i++)
            {
                Console.Write("{0:0.0000}, ", x[i]);
            }

            Console.WriteLine();
            return m;
        }

        #region PlotSplineSolution

        private static void PlotSplineSolution(string title, float[] x, float[] y, float[] xs, float[] ys, string path, float[] qPrime = null)
        {
            var chart = new Chart();
            chart.Size = new Size(4024, 2000);
            chart.Titles.Add(title);
            chart.Legends.Add(new Legend("Legend"));

            ChartArea ca = new ChartArea("DefaultChartArea");
            ca.AxisX.Title = "X(파장)";
            ca.AxisY.Title = "Y(N)";
            chart.ChartAreas.Add(ca);

            Series s1 = CreateSeries(chart, "Spline", CreateDataPoints(xs, ys), Color.Blue, MarkerStyle.None);
            //Series s2 = CreateSeries(chart, "Original", CreateDataPoints(x, y), Color.Green, MarkerStyle.Diamond);

            //chart.Series.Add(s2);
            chart.Series.Add(s1);

 

            ca.RecalculateAxesScale();
            ca.AxisX.Minimum = Math.Floor(ca.AxisX.Minimum);
            ca.AxisX.Maximum = Math.Ceiling(ca.AxisX.Maximum);
            int nIntervals = (x.Length - 1);


            nIntervals = Math.Max(4, nIntervals);
            ca.AxisX.Interval = 30;//(ca.AxisX.Maximum - ca.AxisX.Minimum) / nIntervals;

            // Save
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            {
                chart.SaveImage(fs, ChartImageFormat.Png);
            }
        }

        private static List<DataPoint> CreateDataPoints(float[] x, float[] y)
        {
            Debug.Assert(x.Length == y.Length);
            List<DataPoint> points = new List<DataPoint>();

            for (int i = 0; i < x.Length; i++)
            {
                points.Add(new DataPoint(x[i], y[i]));
            }

            return points;
        }

        private static Series CreateSeries(Chart chart, string seriesName, IEnumerable<DataPoint> points, Color color, MarkerStyle markerStyle = MarkerStyle.None)
        {
            var s = new Series()
            {
                XValueType = ChartValueType.Double,
                YValueType = ChartValueType.Double,
                Legend = chart.Legends[0].Name,
                IsVisibleInLegend = true,
                ChartType = SeriesChartType.Spline,
                Name = seriesName,
                ChartArea = chart.ChartAreas[0].Name,
                MarkerStyle = markerStyle,
                Color = color,
                MarkerSize = 8
            };

            foreach (var p in points)
            {
                s.Points.Add(p);
            }

            return s;
        }

        #endregion
    }
}
