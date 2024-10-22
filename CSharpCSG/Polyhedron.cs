﻿/*
 * Polyhedron.cs
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
using System.Collections.Generic;
using System.Linq;

namespace CSharpCSG
{
    /// <summary>
    /// Polyhedron.
    /// </summary>
    /// 
    /// author: Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    public class Polyhedron : IPrimitive
    {

        private readonly PropertyStorage properties = new PropertyStorage();

        private readonly List<IVector3d> points = new List<IVector3d>();
        private readonly List<List<int>> faces = new List<List<int>>();

        /// <summary>
        /// Constructor. Creates a polyhedron defined by a list of points and a list
        /// of faces.
        /// 
        /// </summary>
        /// <param name="points">points (<see cref="Vector3d"/> list)</param>
        /// <param name="faces">list of faces (list of point index lists)</param>
        /// 
        public Polyhedron(List<IVector3d> points, List<List<int>> faces)
        {
            this.points.AddRange(points);
            this.faces.AddRange(faces);
        }

        /// <summary>
        /// Constructor. Creates a polyhedron defined by a list of points and a list
        /// of faces.
        /// </summary>
        /// 
        /// <param name="points">points (<see cref="IVector3d"/> array)</param>
        /// <param name="faces">list of faces (array of point index arrays)</param>
        /// 
        public Polyhedron(IVector3d[] points, int[][] faces)
        {
            this.points.AddRange(points.ToList());

            foreach (int[] list in faces)
            {
                this.faces.Add(list.ToList());
            }

        }


        public List<Polygon> toPolygons()
        {

            Func<int, IVector3d> indexToPoint = i => points[i].clone();
            Func<List<int>, Polygon> faceListToPolygon
                    = faceList => Polygon.fromPoints(faceList.Select(indexToPoint).ToList(), properties);

            return faces.Select(faceListToPolygon).ToList();
        }

        public PropertyStorage getProperties()
        {
            return properties;
        }

    }
}
