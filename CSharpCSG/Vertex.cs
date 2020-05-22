/*
 * Vertex.cs
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
using System.Text;
using CSharpVecMath;

namespace CSharpCSG
{
    /// <summary>
    /// Represents a vertex of a polygon. This class provides <see cref="#normal"/> so
    /// primitives like <see cref="Cube"/> can return a smooth vertex normal, but
    /// <see cref="#normal"/> is not used anywhere else.
    /// </summary>
    public class Vertex : IEquatable<Vertex>
    {

        /// <summary>
        /// Vertex position.
        /// </summary>
        public IVector3d pos;

        /// <summary>
        /// Normal.
        /// </summary>
        public IVector3d normal;

        private double weight = 1.0;

        /// <summary>
        /// Constructor. Creates a vertex.
        /// </summary>
        /// <param name="pos">position</param>
        /// <param name="normal">normal</param>
        /// 
        public Vertex(IVector3d pos, IVector3d normal)
        {
            this.pos = pos;
            this.normal = normal;
        }


        /// <summary>
        /// Constructor. Creates a vertex.
        /// </summary>
        /// <param name="pos">position</param>
        /// <param name="normal">normal</param>
        /// <param name="weight">weight</param>
        /// 
        private Vertex(IVector3d pos, IVector3d normal, double weight)
        {
            this.pos = pos;
            this.normal = normal;
            this.weight = weight;
        }

       
        public Vertex clone()
        {
            return new Vertex(pos.clone(), normal.clone(), weight);
        }

        /// <summary>
        /// Inverts all orientation-specific data. (e.g. vertex normal).
        /// </summary>
        public void flip()
        {
            normal = normal.negated();
        }

        /// <summary>
        /// Create a new vertex between this vertex and the specified vertex by
        /// linearly interpolating all properties using a parameter t.
        /// </summary>
        /// <param name="other">vertex</param>
        /// <param name="t">interpolation parameter</param>
        /// <returns>a new vertex between this and the specified vertex</returns>
        /// 
        public Vertex interpolate(Vertex other, double t)
        {
            return new Vertex(pos.lerp(other.pos, t),
                    normal.lerp(other.normal, t));
        }

        /// <summary>
        /// Returns this vertex in STL string format.
        /// </summary>
        /// <returns>this vertex in STL string format</returns>
        /// 
        public string toStlString()
        {
            return "vertex " + this.pos.toStlString();
        }

        /// <summary>
        /// Returns this vertex in STL string format.
        /// </summary>
        /// <param name="sb">string builder</param>
        /// <returns>the specified string builder</returns>
        /// 
        public StringBuilder toStlString(StringBuilder sb)
        {
            sb.Append("vertex ");
            return this.pos.toStlString(sb);
        }

        /// <summary>
        /// Returns this vertex in OBJ string format.
        /// </summary>
        /// <param name="sb">string builder</param>
        /// <returns>the specified string builder</returns>
        /// 
        public StringBuilder toObjString(StringBuilder sb)
        {
            sb.Append("v ");
            return this.pos.toObjString(sb).Append("\n");
        }

        /// <summary>
        /// Returns this vertex in OBJ string format.
        /// </summary>
        /// <returns>this vertex in OBJ string format</returns>
        /// 
        public string toObjString()
        {
            return toObjString(new StringBuilder()).ToString();
        }

        /// <summary>
        /// Applies the specified transform to this vertex.
        /// </summary>
        /// <param name="transform">the transform to apply</param>
        /// <returns>this vertex</returns>
        /// 
        public Vertex transform(Transform transform)
        {
            pos = pos.transformed(transform, weight);
            return this;
        }

        /// <summary>
        /// Applies the specified transform to a copy of this vertex.
        /// </summary>
        /// <param name="transform">the transform to apply</param>
        /// <returns>a copy of this transform</returns>
        /// 
        public Vertex transformed(Transform transform)
        {
            return clone().transform(transform);
        }

        /// <summary>
        /// </summary>
        /// <returns>the weight</returns>
        public double getWeight()
        {
            return weight;
        }

        /// <summary>
        /// </summary>
        /// <param name="weight">the weight to set</param>
        /// 
        public void setWeight(double weight)
        {
            this.weight = weight;
        }

        
        public override string ToString()
        {
            return pos.ToString();
        }


        public override int GetHashCode()
        {
            int hash = 7;
            hash = 53 * hash + pos.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            Vertex v = obj as Vertex;
            if (v != null)
            {
                return Equals(v);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Vertex other)
        {
            return pos.Equals(other.pos);
        }

        public static bool operator ==(Vertex v1, Vertex v2)
        {
            if (object.ReferenceEquals(v1, v2)) return true;
            if (object.ReferenceEquals(v1, null)) return false;
            if (object.ReferenceEquals(v2, null)) return false;

            return v1.Equals(v2);
        }

        public static bool operator !=(Vertex v1, Vertex v2)
        {
            if (object.ReferenceEquals(v1, v2)) return false;
            if (object.ReferenceEquals(v1, null)) return true;
            if (object.ReferenceEquals(v2, null)) return true;

            return !v1.Equals(v2);
        }


    }
}
