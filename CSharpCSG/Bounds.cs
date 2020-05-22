/*
 * Bounds.cs
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

using System;
using CSharpVecMath;

namespace CSharpCSG
{
    /// <summary>
    /// Bounding box for CSGs.
    /// </summary>
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class Bounds
    {

        private readonly IVector3d center;
        private readonly IVector3d bounds;
        private readonly IVector3d min;
        private readonly IVector3d max;
        private CSG csg;
        private readonly Cube cube;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        /// <param name="min">min x,y,z values</param>
        /// <param name="max">max x,y,z values</param>
        /// 
        public Bounds(IVector3d min, IVector3d max)
        {
            this.center = Vector3d.xyz(
                    (max.x() + min.x()) / 2,
                    (max.y() + min.y()) / 2,
                    (max.z() + min.z()) / 2);

            this.bounds = Vector3d.xyz(
                    Math.Abs(max.x() - min.x()),
                    Math.Abs(max.y() - min.y()),
                    Math.Abs(max.z() - min.z()));

            this.min = min.clone();
            this.max = max.clone();

            cube = new Cube(center, bounds);

        }

        public Bounds clone()
        {
            return new Bounds(min.clone(), max.clone());
        }

        /// <summary>
        /// Returns the position of the center.
        /// </summary>
        /// 
        /// <returns>the center position</returns>
        /// 
        public IVector3d getCenter()
        {
            return center;
        }

        /// <summary>
        /// Returns the bounds (width,height,depth).
        /// </summary>
        /// <returns>the bounds (width,height,depth)</returns>
        /// 
        public IVector3d getBounds()
        {
            return bounds;
        }

        /// <summary>
        /// Returns this bounding box as csg.
        /// </summary>
        /// <returns>this bounding box as csg</returns>
        /// 
        public CSG toCSG()
        {

            if (csg == null)
            {
                csg = cube.toCSG();
            }

            return csg;
        }

        /// <summary>
        /// Returns this bounding box as cube.
        /// </summary>
        /// <returns>this bounding box as cube</returns>
        /// 
        public Cube toCube()
        {
            return cube;
        }

        /// <summary>
        /// Indicates whether the specified vertex is contained within this bounding
        /// box (check includes box boundary).
        /// </summary>
        /// <param name="v">vertex to check</param>
        /// <returns><c>true</c> if the vertex is contained within this bounding box;
        /// <c>false</c> otherwise</returns>
        /// 
        public bool contains(Vertex v)
        {
            return contains(v.pos);
        }

        /// <summary>
        /// Indicates whether the specified point is contained within this bounding
        /// box (check includes box boundary).
        /// </summary>
        /// <param name="v">vertex to check</param>
        /// <returns><c>true</c> if the point is contained within this bounding box;
        /// <c>false</c> otherwise
        /// </returns>
        public bool contains(IVector3d v)
        {
            bool inX = min.x() <= v.x() && v.x() <= max.x();
            bool inY = min.y() <= v.y() && v.y() <= max.y();
            bool inZ = min.z() <= v.z() && v.z() <= max.z();

            return inX && inY && inZ;
        }

        /// <summary>
        /// Indicates whether the specified polygon is contained within this bounding
        /// box (check includes box boundary).
        /// </summary>
        /// <param name="p">polygon to check</param>
        /// <returns><c>true</c> if the polygon is contained within this bounding
        /// box; <c>false</c> otherwise</returns>
        /// 
        public bool contains(Polygon p)
        {
            return p.vertices.TrueForAll(v => contains(v));
        }

        /// <summary>
        /// Indicates whether the specified polygon intersects with this bounding box
        /// (check includes box boundary).
        /// </summary>
        /// <param name="p">polygon to check</param>
        /// <returns><c>true</c> if the polygon intersects this bounding box;
        /// <c>false</c> otherwise</returns>
        /// @deprecated not implemented yet
        /// 
        public bool intersects(Polygon p)
        {
            throw new NotSupportedException("Implementation missing!");
        }

        /// <summary>
        /// Indicates whether the specified bounding box intersects with this
        /// bounding box (check includes box boundary).
        /// </summary>
        /// <param name="b">box to check</param>
        /// <returns><c>true</c> if the bounding box intersects this bounding box;
        /// <c>false</c> otherwise</returns>
        /// 
        public bool intersects(Bounds b)
        {

            if (b.getMin().x() > this.getMax().x() || b.getMax().x() < this.getMin().x())
            {
                return false;
            }
            if (b.getMin().y() > this.getMax().y() || b.getMax().y() < this.getMin().y())
            {
                return false;
            }
            if (b.getMin().z() > this.getMax().z() || b.getMax().z() < this.getMin().z())
            {
                return false;
            }

            return true;

        }

        /// 
        /// <returns>the min x,y,z values</returns>
        /// 
        public IVector3d getMin()
        {
            return min;
        }

        /// 
        /// <returns>the max x,y,z values</returns>
        /// 
        public IVector3d getMax()
        {
            return max;
        }


        public override string ToString()
        {
            return "[center: " + center + ", bounds: " + bounds + "]";
        }

    }
}
