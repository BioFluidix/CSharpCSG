/*
 * XModifier.cs
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
using System;

namespace CSharpCSG
{
    /// <summary>
    /// Modifies along x axis.
    /// </summary>
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class XModifier : WeightFunction
    {

        private Bounds bounds;
        private double min = 0;
        private double max = 1.0;

        private double sPerUnit;
        private bool centered;


        /// <summary>
        /// Constructor.
        /// </summary>
        public XModifier()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        /// <param name="centered">defines whether to center origin at the csg location</param>
        /// 
        public XModifier(bool centered)
        {
            this.centered = centered;
        }


        public double eval(IVector3d pos, CSG csg)
        {

            if (bounds == null)
            {
                this.bounds = csg.getBounds();
                sPerUnit = (max - min) / (bounds.getMax().x() - bounds.getMin().x());
            }

            double s = sPerUnit * (pos.x() - bounds.getMin().x());

            if (centered)
            {
                s = s - (max - min) / 2.0;

                s = Math.Abs(s) * 2;
            }

            return s;
        }

    }
}
