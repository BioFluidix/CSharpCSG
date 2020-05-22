/*
 * IPrimitive.cs
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


using System.Collections.Generic;

namespace CSharpCSG
{

    public static class PrimitiveExtensions
    {
        /// <summary>
        /// Returns this primitive as <see cref="CSG"/>.
        /// </summary>
        /// <returns>this primitive as <see cref="CSG"/></returns>
        /// 
        public static CSG toCSG(this IPrimitive primitive)
        {
            return CSG.fromPolygons(primitive.getProperties(), primitive.toPolygons());
        }

    }

    /// <summary>
    /// A primitive geometry.
    /// </summary>
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public interface IPrimitive
    {

        /// <summary>
        /// Returns the polygons that define this primitive.
        /// </summary>
        /// <b>Note:</b> this method computes the polygons each time this method is
        /// called. The polygons can be cached inside a <see cref="CSG"/> object.
        /// 
        /// <returns>a list of polygons that define this primitive</returns>
        /// 
        List<Polygon> toPolygons();


        /// <summary>
        /// Returns the property storage of this primitive.
        /// </summary>
        /// <returns>the property storage of this primitive</returns>
        /// 
        PropertyStorage getProperties();
    }
}
