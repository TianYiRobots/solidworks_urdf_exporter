// Standalone numeric verification of the new per-part parallel-axis combination.
// Replicates the math in ExportHelperExtension.ComputeInertialProperties without MathNet.
using System;

public static class V
{
    public static double[,] Mul44(double[,] A, double[,] B)
    {
        double[,] C = new double[4, 4];
        for (int i = 0; i < 4; i++) for (int j = 0; j < 4; j++)
        { double s = 0; for (int k = 0; k < 4; k++) s += A[i, k] * B[k, j]; C[i, j] = s; }
        return C;
    }
    public static double[] Mul4(double[,] T, double[] p)
    {
        double[] r = new double[4];
        for (int i = 0; i < 4; i++)
        { double s = 0; for (int j = 0; j < 4; j++) s += T[i, j] * p[j]; r[i] = s; }
        return r;
    }
    public static double[,] Inv4(double[,] T)
    {
        double[,] Ri = new double[4, 4];
        double[] t = new double[3];
        for (int i = 0; i < 3; i++) { t[i] = T[i, 3]; for (int j = 0; j < 3; j++) Ri[i, j] = T[j, i]; }
        for (int i = 0; i < 3; i++)
        { double s = 0; for (int j = 0; j < 3; j++) s += T[j, i] * t[j]; Ri[i, 3] = -s; }
        Ri[3, 3] = 1;
        return Ri;
    }
    public static double[,] Mul3(double[,] A, double[,] B)
    {
        double[,] C = new double[3, 3];
        for (int i = 0; i < 3; i++) for (int j = 0; j < 3; j++)
        { double s = 0; for (int k = 0; k < 3; k++) s += A[i, k] * B[k, j]; C[i, j] = s; }
        return C;
    }
    public static double[,] T3(double[,] A)
    {
        double[,] B = new double[3, 3];
        for (int i = 0; i < 3; i++) for (int j = 0; j < 3; j++) B[i, j] = A[j, i];
        return B;
    }

    static int fails = 0;
    static void Check(bool cond, string label, string got)
    {
        Console.WriteLine((cond ? "  [PASS] " : "  [FAIL] ") + label + (cond ? "" : "  got=" + got));
        if (!cond) fails++;
    }
    static bool Near(double a, double b) { return Math.Abs(a - b) < 1e-9; }

    public static void Main()
    {
        Console.WriteLine("== Parallel-axis combination (mirrors new ComputeInertialProperties) ==");

        // Two point "bodies": m1=2kg at world (0.3,0,0), m2=1kg at world (-0.1,0.2,0).
        // Part-local COM = part origin; JPart = zero (point masses). Link frame = world.
        var T1 = new double[,] { {1,0,0,0.3}, {0,1,0,0}, {0,0,1,0}, {0,0,0,1} };
        var T2 = new double[,] { {1,0,0,-0.1}, {0,1,0,0.2}, {0,0,1,0}, {0,0,0,1} };
        double m1 = 2.0, m2 = 1.0;
        double[] w1 = Mul4(T1, new double[] {0,0,0,1});
        double[] w2 = Mul4(T2, new double[] {0,0,0,1});
        double total = m1 + m2;
        double[] comW = new double[] {
            (m1*w1[0]+m2*w2[0])/total,
            (m1*w1[1]+m2*w2[1])/total,
            (m1*w1[2]+m2*w2[2])/total };
        Check(Near(comW[0], (0.3*2.0-0.1*1.0)/3.0), "combined COM x", comW[0].ToString());
        Check(Near(comW[1], 0.2*1.0/3.0), "combined COM y", comW[1].ToString());

        double[] d1 = new double[] { w1[0]-comW[0], w1[1]-comW[1], w1[2]-comW[2] };
        double[] d2 = new double[] { w2[0]-comW[0], w2[1]-comW[1], w2[2]-comW[2] };
        double[,] JWorld = new double[3,3];
        double[] ms = new double[] { m1, m2 };
        double[][] ds = new double[][] { d1, d2 };
        for (int n = 0; n < 2; n++)
        {
            double m = ms[n]; double[] d = ds[n];
            double d2v = d[0]*d[0]+d[1]*d[1]+d[2]*d[2];
            for (int i=0;i<3;i++) for (int j=0;j<3;j++)
            {
                double off = (i==j ? d2v : 0.0) - d[i]*d[j];
                JWorld[i,j] += m*off;
            }
        }
        Check(Near(JWorld[0,0], m1*(d1[1]*d1[1]) + m2*(d2[1]*d2[1])), "Ixx", JWorld[0,0].ToString());
        Check(Near(JWorld[1,1], m1*(d1[0]*d1[0]) + m2*(d2[0]*d2[0])), "Iyy", JWorld[1,1].ToString());
        Check(Near(JWorld[2,2], m1*(d1[0]*d1[0]+d1[1]*d1[1]) + m2*(d2[0]*d2[0]+d2[1]*d2[1])), "Izz", JWorld[2,2].ToString());
        Check(Near(JWorld[0,1], -(m1*d1[0]*d1[1] + m2*d2[0]*d2[1])), "Ixy", JWorld[0,1].ToString());

        Console.WriteLine("== Tensor rotation R J R^T under a 90deg Y rotation ==");
        double[,] Jp = new double[,] { {1,0,0},{0,2,0},{0,0,3} };
        double[,] Ry = new double[,] { {0,0,1},{0,1,0},{-1,0,0} };
        double[,] Jr = Mul3(Mul3(Ry, Jp), T3(Ry));
        Check(Near(Jr[0,0], 3.0), "rotated Ixx=old Izz", Jr[0,0].ToString());
        Check(Near(Jr[1,1], 2.0), "rotated Iyy unchanged", Jr[1,1].ToString());
        Check(Near(Jr[2,2], 1.0), "rotated Izz=old Ixx", Jr[2,2].ToString());

        Console.WriteLine("== Link-frame localization: TLink = Rx(-90) (SW Y-up -> URDF Z-up) ==");
        var TLink = new double[,] { {1,0,0,0},{0,0,1,0},{0,-1,0,0},{0,0,0,1} };
        double[] comLink = Mul4(Inv4(TLink), new double[] {0.1,0.2,0.3,1});
        Check(Near(comLink[0],0.1)&&Near(comLink[1],0.3)&&Near(comLink[2],-0.2),
            "COM localized to link frame",
            "("+comLink[0]+","+comLink[1]+","+comLink[2]+")");

        Console.WriteLine(fails == 0 ? "ALL PASS" : (fails + " FAILED"));
        Environment.ExitCode = fails == 0 ? 0 : 1;
    }
}
