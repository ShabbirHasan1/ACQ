﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACQ.Math
{
    #region cephes
    /*
        
        This file is the original README from that page, which also states the license.

           Some software in this archive may be from the book _Methods and
        Programs for Mathematical Functions_ (Prentice-Hall or Simon & Schuster
        International, 1989) or from the Cephes Mathematical Library, a
        commercial product. In either event, it is copyrighted by the author.
        What you see here may be used freely but it comes with no support or
        guarantee.

           The two known misprints in the book are repaired here in the
        source listings for the gamma function and the incomplete beta
        integral.


           Stephen L. Moshier
           moshier@na-net.ornl.gov
     * */
    #endregion
    /// <summary>
    /// Special functions, adapted from Cephes (subset)
    /// https://github.com/jeremybarnes/cephes
    /// http://www.netlib.org/cephes/
    /// function names are the same as Cephes
    /// </summary>
    public class Special
    {
        #region const
        private const double MACHEP = 1.11022302462515654042E-16;
        private const double MAXLOG = 7.09782712893383996732E2;
        private const double MINLOG = -7.451332191019412076235E2;
        private const double MAXGAM = 171.624376956302725;
        private const double SQTPI = 2.50662827463100050242E0;
        private const double SQRTH = 7.07106781186547524401E-1;
        private const double SQRT2 = 1.41421356237309504880;
        private const double LOGPI = 1.14472988584940017414;
        private const double LS2PI = 0.91893853320467274178;// log( sqrt( 2*pi ) )
        private const double SQT2PI_INV = 0.39894228040143267793994605993438; //1/sqrt(2*pi)

        private const double MAXLGM = 2.556348e305;

        private const double big = 4.503599627370496e15;
        private const double biginv =  2.22044604925031308085e-16;

        #endregion 

        #region polycoeffs
        private static readonly double[] m_gamma_P = {
          1.60119522476751861407E-4,
          1.19135147006586384913E-3,
          1.04213797561761569935E-2,
          4.76367800457137231464E-2,
          2.07448227648435975150E-1,
          4.94214826801497100753E-1,
          9.99999999999999996796E-1	};
        private static readonly double[] m_gamma_Q = {
        -2.31581873324120129819E-5,
         5.39605580493303397842E-4,
        -4.45641913851797240494E-3,
         1.18139785222060435552E-2,
         3.58236398605498653373E-2,
        -2.34591795718243348568E-1,
         7.14304917030273074085E-2,
         1.00000000000000000320E0};
        private static readonly double[] m_gamma_STIR = {
        7.87311395793093628397E-4,
        -2.29549961613378126380E-4,
        -2.68132617805781232825E-3,
        3.47222221605458667310E-3,
        8.33333333333482257126E-2};
        private static readonly double[] m_gamma_A = {
        8.11614167470508450300E-4,
        -5.95061904284301438324E-4,
        7.93650340457716943945E-4,
        -2.77777777730099687205E-3,
        8.33333333333331927722E-2};
        private static readonly double[] m_gamma_B = {
		-1.37825152569120859100E3,
		-3.88016315134637840924E4,
		-3.31612992738871184744E5,
		-1.16237097492762307383E6,
		-1.72173700820839662146E6,
		-8.53555664245765465627E5};
        private static readonly double[] m_gamma_C = {
		/* 1.00000000000000000000E0, */
		-3.51815701436523470549E2,
		-1.70642106651881159223E4,
		-2.20528590553854454839E5,
		-1.13933444367982507207E6,
		-2.53252307177582951285E6,
		-2.01889141433532773231E6};
        private static readonly double[] m_ndtr_P = {
		2.46196981473530512524E-10,
		5.64189564831068821977E-1,
		7.46321056442269912687E0,
		4.86371970985681366614E1,
		1.96520832956077098242E2,
		5.26445194995477358631E2,
		9.34528527171957607540E2,
		1.02755188689515710272E3,
		5.57535335369399327526E2};
        private static readonly double[] m_ndtr_Q = {
		//1.0
		1.32281951154744992508E1,
		8.67072140885989742329E1,
		3.54937778887819891062E2,
		9.75708501743205489753E2,
		1.82390916687909736289E3,
		2.24633760818710981792E3,
		1.65666309194161350182E3,
		5.57535340817727675546E2};
        private static readonly double[] m_ndtr_R = {
		5.64189583547755073984E-1,
		1.27536670759978104416E0,
		5.01905042251180477414E0,
		6.16021097993053585195E0,
		7.40974269950448939160E0,
		2.97886665372100240670E0};
        private static readonly double[] m_ndtr_S = {
	    //1.00000000000000000000E0, 
	    2.26052863220117276590E0,
	    9.39603524938001434673E0,
	    1.20489539808096656605E1,
	    1.70814450747565897222E1,
	    9.60896809063285878198E0,
	    3.36907645100081516050E0};
        private static readonly double[] m_ndtr_T = {
		9.60497373987051638749E0,
        9.00260197203842689217E1,
        2.23200534594684319226E3,
        7.00332514112805075473E3,
        5.55923013010394962768E4	};
        private static readonly double[] m_ndtr_U = {
		//1.00000000000000000000E0,
		3.35617141647503099647E1,
		5.21357949780152679795E2,
		4.59432382970980127987E3,
		2.26290000613890934246E4,
		4.92673942608635921086E4};
        #endregion 



        /// <summary>
        /// chi-square function (left hand tail).
        /// </summary>
        public static double chisq(double df, double x)
        {
            if (x < 0.0 || df < 1.0)
            {
                return 0.0;
            }
            return igam( df/2.0, x/2.0 );
        }
        /// <summary>
        /// chi-square function (right hand tail)
        /// </summary>
        public static double chisqc(double df, double x)
        {
            if (x < 0.0 || df < 1.0) return 0.0;

            return igamc(df / 2.0, x / 2.0);
        }

        /// <summary>
        /// Returns the sum of the first k terms of the Poisson distribution
        /// </summary>
        /// <param name="k"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double poisson(int k, double x)
        {
            if (k < 0 || x < 0) 
                return 0.0;

            return igamc((double)(k + 1), x);
        }

        public static double poissonc(int k, double x)
        {
            if (k < 0 || x < 0) 
                return 0.0;

            return igam((double)(k + 1), x);
        }

        public static double fac(double x)
        {
            if (x < 0.0)
            {
                return Double.NaN;
            }
            else
            {
                return gamma(x + 1.0);
            }
        }

        #region gamma
        /// <summary>
        /// Returns the gamma function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double gamma(double x)
        {
            int sgngam;
            return gamma(x, out sgngam);
        }
        /// <summary>
        /// Returns the gamma function
        /// </summary>
        private static double gamma(double x, out int sgngam)
        {
            sgngam = 1;
            if (Double.IsNaN(x))
            {
                return Double.NaN;
            }

            if (Double.IsPositiveInfinity(x))
            {
                return Double.PositiveInfinity;
            }

            if (Double.IsNegativeInfinity(x))
            {
                return Double.NaN;
            }
            int i;
            double p, z;

            double q = System.Math.Abs(x);

            if (q > 33.0)
            {
                if (x < 0.0)
                {
                    p = System.Math.Floor(q);
                    if (p == q)
                    {
                        return Double.NaN; //gamma: overflow
                    }
                    i = (int)p;
                    if ((i & 1) == 0)
                    {
                        sgngam = -1;
                    }
                    z = q - p;
                    if (z > 0.5)
                    {
                        p += 1.0;
                        z = q - p;
                    }
                    z = q * System.Math.Sin(System.Math.PI * z);
                    if (z == 0.0)
                    {
                        return sgngam > 0 ? Double.PositiveInfinity : Double.NegativeInfinity;//"gamma: overflow";
                    }
                    z = System.Math.Abs(z);
                    z = System.Math.PI / (z * stirf(q));
                }
                else
                {
                    z = stirf(x);
                }
                return sgngam * z;
            }

            z = 1.0;
            while (x >= 3.0)
            {
                x -= 1.0;
                z *= x;
            }

            while (x < 0.0)
            {
                if (x == 0.0)
                {
                    return Double.PositiveInfinity;//"gamma: overflow");
                }
                else if (x > -1.0E-9)
                {
                    return (z / ((1.0 + 0.5772156649015329 * x) * x));
                }
                z /= x;
                x += 1.0;
            }

            while (x < 2.0)
            {
                if (x == 0.0)
                {
                    return Double.PositiveInfinity;//"gamma: singular"
                }
                else if (x < 1.0E-9)
                {
                    return (z / ((1.0 + 0.5772156649015329 * x) * x));
                }
                z /= x;
                x += 1.0;
            }

            if (x == 2.0)
            {
                return z;
            }

            x -= 2.0;
            p = polevl(x, m_gamma_P, 6);
            q = polevl(x, m_gamma_Q, 7);
            return z * p / q;
        }

        /// <summary>
        /// Gamma function computed by Stirling's formula, The polynomial STIR is valid for 33 <= x <= 172.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double stirf(double x)
        {
            const double MAXSTIR = 143.01608;

            double w = 1.0 / x;
            double y = exp(x);

            w = 1.0 + w * polevl(w, m_gamma_STIR, 4);

            if (x > MAXSTIR)
            {
                // Avoid overflow in Math.Pow() 
                double v = pow(x, 0.5 * x - 0.25);
                y = v * (v / y);
            }
            else
            {
                y = pow(x, x - 0.5) / y;
            }
            y = SQTPI * y * w;
            return y;
        }

        /// <summary>
        /// complemented incomplete gamma function.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double igamc(double a, double x)
        {           
            double ans, ax, c, yc, r, t, y, z;
            double pk, pkm1, pkm2, qk, qkm1, qkm2;

            if (x <= 0 || a <= 0)
            {
                return 1.0;
            }

            if (x < 1.0 || x < a)
            {
                return 1.0 - igam(a, x);
            }

            ax = a * log(x) - x - lgam(a);

            if (ax < -MAXLOG)
            {
                return 0.0;//UNDERFLOW 
            }

            ax = exp(ax);

            // continued fraction 
            y = 1.0 - a;
            z = x + y + 1.0;
            c = 0.0;
            pkm2 = 1.0;
            qkm2 = x;
            pkm1 = x + 1.0;
            qkm1 = z * x;
            ans = pkm1 / qkm1;

            do
            {
                c += 1.0;
                y += 1.0;
                z += 2.0;
                yc = y * c;
                pk = pkm1 * z - pkm2 * yc;
                qk = qkm1 * z - qkm2 * yc;
                if (qk != 0)
                {
                    r = pk / qk;
                    t = System.Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                    t = 1.0;

                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if (fabs(pk) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
            } while (t > MACHEP);

            return ans * ax;
        }


        /// <summary>
        /// Incomplete gamma integral.
        /// </summary>
        public static double igam(double a, double x)
        {
            double ans, ax, c, r;

            if (x <= 0 || a <= 0)
            {
                return 0.0;
            }

            if (x > 1.0 && x > a)
            {
                return 1.0 - igamc(a, x);
            }

            // Compute  x**a * exp(-x) / gamma(a)  
            ax = a * log(x) - x - lgam(a);
            if (ax < -MAXLOG)
            {
                return (0.0);
            }

            ax = exp(ax);

            // power series 
            r = a;
            c = 1.0;
            ans = 1.0;

            do
            {
                r += 1.0;
                c *= x / r;
                ans += c;
            } while (c / ans > MACHEP);

            return ans * ax / a;
        }

        public static double lgam(double x)
        {
            int sgngam;
            return lgam(x, out sgngam);
        }
        /// <summary>
        /// Logarithm of gamma function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double lgam(double x, out int sgngam)
        {
            double p, q, u, w, z;
            int i;

            sgngam = 1;

            if (Double.IsNaN(x))
            {
                return Double.NaN;
            }

            if (Double.IsInfinity(x))
            {
                return Double.PositiveInfinity; 
            }

            if (x < -34.0)
            {
                q = -x;
                w = lgam(q, out sgngam);
                p = System.Math.Floor(q);
                if (p == q)
                {
                    return Double.PositiveInfinity; //lgam: Overflow
                }

                i = (int)p;
                if ((i & 1) == 0)
                    sgngam = -1;
                else
                    sgngam = 1;

                z = q - p;
                if (z > 0.5)
                {
                    p += 1.0;
                    z = p - q;
                }
                z = q * System.Math.Sin(System.Math.PI * z);
                if (z == 0.0) 
                {
                    return Double.PositiveInfinity;
                }
                z = LOGPI - log(z) - w;
                return z;
            }

            if (x < 13.0)
            {
                z = 1.0;
                p = 0.0;
                u = x;
                while (u >= 3.0)
                {
                    p -= 1.0;
                    u = x + p;
                    z *= u;
                }
                while (u < 2.0)
                {
                    if (x == 0.0)
                        return Double.PositiveInfinity;
                    z /= u;
                    p += 1.0;
                    u = x + p;
                }
                if (z < 0.0)
                {
                    sgngam = -1;
                    z = -z;
                }
                else
                {
                    sgngam = 1;
                }

                if (u == 2.0)
                {
                    return log(z);
                }

                p -= 2.0;
                x = x + p;
                p = x * polevl(x, m_gamma_B, 5) / p1evl(x, m_gamma_C, 6);

                return (log(z) + p);
            }

            if (x > MAXLGM)
            {
                return sgngam == 1 ? Double.PositiveInfinity : Double.NegativeInfinity;
            }

            q = (x - 0.5) * log(x) - x + LS2PI;
            if (x > 1.0e8)
            {
                return (q);
            }

            p = 1.0 / (x * x);
            if (x >= 1000.0)
            {
                q += ((7.9365079365079365079365e-4 * p
                    - 2.7777777777777777777778e-3) * p
                    + 0.0833333333333333333333) / x;
            }
            else
            {
                q += polevl(p, m_gamma_A, 4) / x;
            }
            return q;
        }
 
        /// <summary>
        /// Gamma distribution function
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double gdtr( double a, double b, double x )
        {
            if( x < 0.0 )
            {
                return 0.0;
            }
            return igam( b, a * x );
        }
        /// <summary>
        /// Returns the integral from x to infinity of the gamma probability density function:
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double gdtrc( double a, double b, double x )
        {
            if( x < 0.0 )
            {
                return 0.0;
            }
            return igamc( b, a * x );
        }
        #endregion 

        #region normal
        /// <summary>
        /// Returns the area under the Gaussian probability density, integrated from minus infinity to x.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double NormalCdf(double x)
        {
            return ndtr(x); 
        }
        public static double NormalPdf(double x)
        {
            return Special.SQT2PI_INV * System.Math.Exp(-0.5* x * x);
        }
        /// <summary>
        /// Returns the area under the Gaussian probability density, integrated from minus infinity to a.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double ndtr(double a)
        {
            double x, y, z;

            x = a * SQRTH;
            z = fabs(x);

            if (z < 1.0) //if( z < SQRTH  ) ?
            {
                y = 0.5 + 0.5 * erf(x);
            }
            else
            {
                y = 0.5 * erfc(z);
                if (x > 0)
                {
                    y = 1.0 - y;
                }
            }

            return y;
        }

        /// <summary>
        /// Complementary error function
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double erfc(double a)
        {
            double x, y, z, p, q;

            if (a < 0.0) 
                x = -a;
            else 
                x = a;

            if (x < 1.0)
            {
                return 1.0 - erf(a);
            }

            z = -a * a;

            if (z < -MAXLOG)
            {
                if (a < 0) 
                    return 2.0;
                else 
                    return 0.0;
            }

            z =  exp(z); // Special.expx2(a, -1);

            if (x < 8.0)
            {
                p = polevl(x, m_ndtr_P, 8);
                q = p1evl(x, m_ndtr_Q, 8);
            }
            else
            {
                p = polevl(x, m_ndtr_R, 5);
                q = p1evl(x, m_ndtr_S, 6);
            }

            y = (z * p) / q;

            if (a < 0)
            {
                y = 2.0 - y;
            }

            if (y == 0.0)
            {
                if (a < 0) 
                    return 2.0;
                else 
                    return 0.0;
            }


            return y;
        }

        /// <summary>
        ///  Error function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double erf(double x)
        {
            double y, z;

            if (fabs(x) > 1.0)
            {
                return (1.0 - erfc(x));
            }
            z = x * x;
            y = x * polevl(z, m_ndtr_T, 4) / p1evl(z, m_ndtr_U, 5);
            return y;
        }

        /// <summary>
        /// Computes y = exp(x*x)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="sign"></param>
        /// <returns></returns>
        private static double expx2 (double x, int sign)
        {
            const double M = 128.0;
            const double MINV = 0.0078125;

            double u, u1, m, f;
            
            x = fabs (x);

            if (sign < 0)
            {
                x = -x;
            }
            
            // Represent x as an exact multiple of M plus a residual.
            //  M is a power of 2 chosen so that exp(m * m) does not overflow
            //  or underflow and so that |x - m| is small.  
            m = MINV * System.Math.Floor(M * x + 0.5);
            f = x - m;

            // x^2 = m^2 + 2mf + f^2 
            u = m * m;
            u1 = 2 * m * f  +  f * f;

            if (sign < 0)
            {
                u = -u;
                u1 = -u1;
            }

            if ((u + u1) > MAXLOG)
            {
                return System.Double.PositiveInfinity;
            }

            // u is exact, u1 is small.  
            u = exp(u) * exp(u1);
            return(u);
        }
        #endregion 

        #region incbet
        /// <summary>
        /// Incomplete beta integral
        /// </summary>
        /// <param name="aa"></param>
        /// <param name="bb"></param>
        /// <param name="xx"></param>
        /// <returns></returns>
        public static double incbet(double aa, double bb, double xx )
        {
            double a, b, t, x, xc, w, y;
            int flag;

            if( aa <= 0.0 || bb <= 0.0 )
                return( 0.0 );

            if( (xx <= 0.0) || ( xx >= 1.0) )
            {
                if( xx == 0.0 )
                    return(0.0);
                if( xx == 1.0 )
                    return( 1.0 );
                return( 0.0 );
            }
            
            flag = 0;
            if( (bb * xx) <= 1.0 && xx <= 0.95)
            {
                t = pseries(aa, bb, xx);
                return t;
            }

            w = 1.0 - xx;

            // Reverse a and b if x is greater than the mean. 
            if( xx > (aa/(aa+bb)) )
            {
                flag = 1;
                a = bb;
                b = aa;
                xc = xx;
                x = w;
            }
            else
            {
                a = aa;
                b = bb;
                xc = w;
                x = xx;
            }

            if( flag == 1 && (b * x) <= 1.0 && x <= 0.95)
            {
                t = pseries(a, b, x);
                if( t <= MACHEP )
                    t = 1.0 - MACHEP;
                else
                    t = 1.0 - t;
                return( t );
            }

            // Choose expansion for better convergence. 
            y = x * (a+b-2.0) - (a-1.0);
            if( y < 0.0 )
                w = incbcf( a, b, x );
            else
                w = incbd( a, b, x ) / xc;
            
            // Multiply w by the factor
            //a      b   _             _     _
            //x  (1-x)   | (a+b) / ( a | (a) | (b) ) .   
            y = a * log(x);
            t = b * log(xc);
            if( (a+b) < MAXGAM && fabs(y) < MAXLOG && fabs(t) < MAXLOG )
            {
                t = pow(xc,b);
                t *= pow(x,a);
                t /= a;
                t *= w;
                t *= gamma(a+b) / (gamma(a) * gamma(b));
                
                if (flag == 1)
                {
                    if (t <= MACHEP)
                        t = 1.0 - MACHEP;
                    else
                        t = 1.0 - t;
                }
                return (t);

            }
            // Resort to logarithms. 
            y += t + lgam(a+b) - lgam(a) - lgam(b);
            y += log(w/a);
            if( y < MINLOG )
                t = 0.0;
            else
                t = exp(y);
            
            if( flag == 1 )
            {
                if( t <= MACHEP )
                    t = 1.0 - MACHEP;
                else
                    t = 1.0 - t;
            }
            return( t );
        }

        /// <summary>
        /// Continued fraction expansion #1 for incomplete beta integral
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double incbcf(double a,double b, double x )
        {
            double xk, pk, pkm1, pkm2, qk, qkm1, qkm2;
            double k1, k2, k3, k4, k5, k6, k7, k8;
            double r, t, ans, thresh;
            int n;

            k1 = a;
            k2 = a + b;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = b - 1.0;
            k7 = k4;
            k8 = a + 2.0;

            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * MACHEP;
            do
            {	
                xk = -( x * k1 * k2 )/( k3 * k4 );
                pk = pkm1 +  pkm2 * xk;
                qk = qkm1 +  qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                xk = ( x * k5 * k6 )/( k7 * k8 );
                pk = pkm1 +  pkm2 * xk;
                qk = qkm1 +  qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if( qk != 0 )
                    r = pk/qk;
                if( r != 0 )
                {
                    t = fabs( (ans - r)/r );
                    ans = r;
                }
                else
                    t = 1.0;

                if( t < thresh )
                   return ans;
                
                k1 += 1.0;
                k2 += 1.0;
                k3 += 2.0;
                k4 += 2.0;
                k5 += 1.0;
                k6 -= 1.0;
                k7 += 2.0;
                k8 += 2.0;

                if( (fabs(qk) + fabs(pk)) > big )
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
                if( (fabs(qk) < biginv) || (fabs(pk) < biginv) )
                {
                    pkm2 *= big;
                    pkm1 *= big;
                    qkm2 *= big;
                    qkm1 *= big;
                }
            }
            while( ++n < 300 );            
            return(ans);
        }
        /// <summary>
        /// Continued fraction expansion #2 for incomplete beta integral
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double incbd(double a, double b, double x )
        {
            double xk, pk, pkm1, pkm2, qk, qkm1, qkm2;
            double k1, k2, k3, k4, k5, k6, k7, k8;
            double r, t, ans, z, thresh;
            int n;

            k1 = a;
            k2 = b - 1.0;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = a + b;
            k7 = a + 1.0;
            k8 = a + 2.0;

            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            z = x / (1.0-x);
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * MACHEP;
            do
            {
                xk = -( z * k1 * k2 )/( k3 * k4 );
                pk = pkm1 +  pkm2 * xk;
                qk = qkm1 +  qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                xk = ( z * k5 * k6 )/( k7 * k8 );
                pk = pkm1 +  pkm2 * xk;
                qk = qkm1 +  qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if( qk != 0 )
                    r = pk/qk;
                if( r != 0 )
                {
                    t = fabs( (ans - r)/r );
                    ans = r;
                }
                else
                    t = 1.0;

                if( t < thresh )
                    return (ans);

                k1 += 1.0;
                k2 -= 1.0;
                k3 += 2.0;
                k4 += 2.0;
                k5 += 1.0;
                k6 += 1.0;
                k7 += 2.0;
                k8 += 2.0;

                if( (fabs(qk) + fabs(pk)) > big )
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
                if( (fabs(qk) < biginv) || (fabs(pk) < biginv) )
                {
                    pkm2 *= big;
                    pkm1 *= big;
                    qkm2 *= big;
                    qkm1 *= big;
                }
            }
            while( ++n < 300 );
            
            return(ans);
        }      
        #endregion

        #region private functions
        private static double fabs(double x)
        {
            return System.Math.Abs(x);
        }
        private static double exp(double x)
        {
            return System.Math.Exp(x);
        }
        private static double log(double x)
        {
            return System.Math.Log(x);
        }
        private static double pow(double x, double a)
        {            
            return System.Math.Pow(x, a);
        }
        ///Power series for incomplete beta integral.  Use when b*x is small and x not too close to 1.
        private static double pseries(double a, double b, double x)
        {
            double s, t, u, v, n, t1, z, ai;
            ai = 1.0 / a;
            u = (1.0 - b) * x;
            v = u / (a + 1.0);
            t1 = v;
            t = u;
            n = 2.0;
            s = 0.0;
            z = MACHEP * ai;
            while (fabs(v) > z)
            {
                u = (n - b) * x / n;
                t *= u;
                v = t / (a + n);
                s += v;
                n += 1.0;
            }
            s += t1;
            s += ai;

            u = a * log(x);
            if ((a + b) < MAXGAM && fabs(u) < MAXLOG)
            {
                t = gamma(a + b) / (gamma(a) * gamma(b));
                s = s * t * pow(x, a);
            }
            else
            {
                t = lgam(a + b) - lgam(a) - lgam(b) + u + log(s);
                if (t < MINLOG)
                    s = 0.0;
                else
                    s = exp(t);
            }
            return (s);
        }
        /// <summary>
        /// Evaluates polynomial of degree n
        /// </summary>
        private static double polevl(double x, double[] coef, int n)
        {
            double ans;

            ans = coef[0];

            for (int i = 1; i <= n; i++)
            {
                ans = ans * x + coef[i];
            }

            return ans;
        }

        /// <summary>
        /// Evaluates polynomial of degree n ( assumtion that coef[n] = 1.0)
        /// </summary>	
        private static double p1evl(double x, double[] coef, int n)
        {
            double ans;

            ans = x + coef[0];

            for (int i = 1; i < n; i++)
            {
                ans = ans * x + coef[i];
            }

            return ans;
        }
        #endregion
    }
}
