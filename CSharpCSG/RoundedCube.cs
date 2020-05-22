/*
 * RoundedCube.cs
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
using System.Collections.Generic;

namespace CSharpCSG
{
    /// 
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class RoundedCube : IPrimitive
    {

        ///
        /// Cube dimensions.
        ///
        private IVector3d dimensions;
        private IVector3d center;
        private bool centered = true;

        private readonly PropertyStorage properties = new PropertyStorage();

        private double _cornerRadius = 0.1;
        private int _resolution = 8;

        ///
        /// Constructor. Creates a new rounded cube with center <c>[0,0,0]</c> and
        /// dimensions <c>[1,1,1]</c>.
        ///
        public RoundedCube()
        {
            center = Vector3d.xyz(0, 0, 0);
            dimensions = Vector3d.xyz(1, 1, 1);
        }

        ///
        /// Constructor. Creates a new rounded cube with center <c>[0,0,0]</c> and
        /// dimensions <c>[size,size,size]</c>.
        ///
        /// <param name="size">size</param>
        ///
        public RoundedCube(double size)
        {
            center = Vector3d.xyz(0, 0, 0);
            dimensions = Vector3d.xyz(size, size, size);
        }

        ///
        /// Constructor. Creates a new rounded cuboid with the specified center and
        /// dimensions.
        ///
        /// <param name="center">center of the cuboid</param>
        /// <param name="dimensions">cube dimensions</param>
        ///
        public RoundedCube(IVector3d center, IVector3d dimensions)
        {
            this.center = center;
            this.dimensions = dimensions;
        }

        ///
        /// Constructor. Creates a new rounded cuboid with center <c>[0,0,0]</c> and
        /// with the specified dimensions.
        ///
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="d">depth</param>
        ///
        public RoundedCube(double w, double h, double d)
                : this(Vector3d.ZERO, Vector3d.xyz(w, h, d)) { }


        public List<Polygon> toPolygons()
        {
            CSG spherePrototype
                    = new Sphere(getCornerRadius(), getResolution() * 2, getResolution()).toCSG();

            double x = dimensions.x() / 2.0 - getCornerRadius();
            double y = dimensions.y() / 2.0 - getCornerRadius();
            double z = dimensions.z() / 2.0 - getCornerRadius();

            CSG sphere1 = spherePrototype.transformed(Transform.unity().translate(-x, -y, -z));
            CSG sphere2 = spherePrototype.transformed(Transform.unity().translate(x, -y, -z));
            CSG sphere3 = spherePrototype.transformed(Transform.unity().translate(x, y, -z));
            CSG sphere4 = spherePrototype.transformed(Transform.unity().translate(-x, y, -z));

            CSG sphere5 = spherePrototype.transformed(Transform.unity().translate(-x, -y, z));
            CSG sphere6 = spherePrototype.transformed(Transform.unity().translate(x, -y, z));
            CSG sphere7 = spherePrototype.transformed(Transform.unity().translate(x, y, z));
            CSG sphere8 = spherePrototype.transformed(Transform.unity().translate(-x, y, z));

            List<Polygon> result = sphere1.union(
                    sphere2, sphere3, sphere4,
                    sphere5, sphere6, sphere7, sphere8).hull().getPolygons();

            Transform locTransform = Transform.unity().translate(center);

            foreach (Polygon p in result)
            {
                p.transform(locTransform);
            }

            if (!centered)
            {

                Transform centerTransform = Transform.unity().
                        translate(dimensions.x() / 2.0,
                                dimensions.y() / 2.0,
                                dimensions.z() / 2.0);

                foreach (Polygon p in result)
                {
                    p.transform(centerTransform);
                }
            }

            return result;
        }

        public PropertyStorage getProperties()
        {
            return properties;
        }

        ///
        /// <returns>the center</returns>
        ///
        public IVector3d getCenter()
        {
            return center;
        }

        ///
        /// <param name="center">the center to set</param>
        ///
        public void setCenter(IVector3d center)
        {
            this.center = center;
        }

        ///
        /// <returns>the dimensions</returns>
        ///
        public IVector3d getDimensions()
        {
            return dimensions;
        }

        ///
        /// <param name="dimensions">the dimensions to set</param>
        ///
        public void setDimensions(IVector3d dimensions)
        {
            this.dimensions = dimensions;
        }

        ///
        /// Defines that this cube will not be centered.
        ///
        /// <returns>this cube</returns>
        ///
        public RoundedCube noCenter()
        {
            centered = false;
            return this;
        }

        ///
        /// <returns>the resolution</returns>
        ///
        public int getResolution()
        {
            return _resolution;
        }

        ///
        /// <param name="resolution">the resolution to set</param>
        ///
        public void setResolution(int resolution)
        {
            _resolution = resolution;
        }

        ///
        /// <param name="resolution">the resolution to set</param>
        /// <returns>this cube</returns>
        ///
        public RoundedCube resolution(int resolution)
        {
            _resolution = resolution;
            return this;
        }

        ///
        /// <returns>the corner radius</returns>
        ///
        public double getCornerRadius()
        {
            return _cornerRadius;
        }

        ///
        /// <param name="cornerRadius">the corner radius to set</param>
        ///
        public void setCornerRadius(double cornerRadius)
        {
            _cornerRadius = cornerRadius;
        }

        ///
        /// <param name="cornerRadius">the corner radius to set</param>
        /// <returns>this cube</returns>
        ///
        public RoundedCube cornerRadius(double cornerRadius)
        {
            _cornerRadius = cornerRadius;
            return this;
        }

    }
}
