/*
 * Matrix3d.cs
 *
 * Copyright 2014-2017 Michael Hoffer <info@michaelhoffer.de>. All rights
 * reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice,
 * this list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY Michael Hoffer <info@michaelhoffer.de> "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL Michael Hoffer <info@michaelhoffer.de> OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
 * OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * The views and conclusions contained in the software and documentation are
 * those of the authors and should not be interpreted as representing official
 * policies, either expressed or implied, of Michael Hoffer
 * <info@michaelhoffer.de>.
 */

using CSharpVecMath;

namespace CSharpCSG
{
    /// <summary>
    /// 3D Matrix3d
    /// </summary>
    /// author: cpoliwoda
    /// 
    public class Matrix3d
    {

        public double m11, m12, m13;
        public double m21, m22, m23;
        public double m31, m32, m33;

        public static readonly Matrix3d ZERO = new Matrix3d(0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static readonly Matrix3d UNITY = new Matrix3d(1, 0, 0, 0, 1, 0, 0, 0, 1);

        public Matrix3d(double m11, double m12, double m13,
                double m21, double m22, double m23,
                double m31, double m32, double m33)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;
            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;
            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
        }

        public override string ToString()
        {
            return "[" + m11 + ", " + m12 + ", " + m13 + "]\n"
                    + "[" + m21 + ", " + m22 + ", " + m23 + "]\n"
                    + "[" + m31 + ", " + m32 + ", " + m33 + "]";
        }

        /// <summary>
        /// Returns the product of this matrix and the specified value.
        /// <b>Note:</b> this matrix is not modified.
        /// </summary>
        /// 
        /// <param name="a">the value</param>
        /// <returns>the product of this matrix and the specified value</returns>
        /// 
        public Matrix3d times(double a)
        {
            return new Matrix3d(
                    m11 * a, m12 * a, m13 * a,
                    m21 * a, m22 * a, m23 * a,
                    m31 * a, m32 * a, m33 * a);
        }

        /// <summary>
        /// Returns the product of this matrix and the specified vector.
        /// <b>Note:</b> the vector is not modified.
        /// </summary>
        /// 
        /// <param name="a">the vector</param>
        /// <returns>the product of this matrix and the specified vector</returns>
        /// 
        public IVector3d times(IVector3d a)
        {
            return Vector3d.xyz(
                    m11 * a.x() + m12 * a.y() + m13 * a.z(),
                    m21 * a.x() + m22 * a.y() + m23 * a.z(),
                    m31 * a.x() + m32 * a.y() + m33 * a.z());
        }

        static double maxOf3Values(double[] values)
        {

            if (values[0] > values[1])
            {

                if (values[0] > values[2])
                {
                    return (values[0]);
                }
                else
                {
                    return (values[2]);
                }

            }
            else
            {

                if (values[1] > values[2])
                {
                    return (values[1]);
                }
                else
                {
                    return (values[2]);
                }

            }

        }

    }
}
