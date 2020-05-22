/*
 * Node.cs
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
using System.Collections.Generic;
using System.Linq;

namespace CSharpCSG
{
    /// <summary>
    /// Holds a node in a BSP tree. A BSP tree is built from a collection of polygons
    /// by picking a polygon to split along. That polygon (and all other coplanar
    /// polygons) are added directly to that node and the other polygons are added to
    /// the front and/or back subtrees. This is not a leafy BSP tree since there is
    /// no distinction between internal and leaf nodes.
    /// </summary>
    public sealed class Node
    {

        /// <summary>
        /// Polygons.
        /// </summary>
        private List<Polygon> polygons;
        /// <summary>
        /// Plane used for BSP.
        /// </summary>
        private Plane plane;
        /// <summary>
        /// Polygons in front of the plane.
        /// </summary>
        private Node front;
        /// <summary>
        /// Polygons in back of the plane.
        /// </summary>
        private Node back;

        /// <summary>
        /// Constructor.
        /// Creates a BSP node consisting of the specified polygons.
        /// </summary>
        /// 
        /// <param name="polygons">polygons</param>
        /// 
        public Node(List<Polygon> polygons)
        {
            this.polygons = new List<Polygon>();
            if (polygons != null)
            {
                this.build(polygons);
            }
        }

        /// <summary>
        /// Constructor. Creates a node without polygons.
        /// </summary>
        public Node() : this(null) {}

        public Node clone()
        {
            Node node = new Node();
            node.plane = this.plane == null ? null : this.plane.clone();
            node.front = this.front == null ? null : this.front.clone();
            node.back = this.back == null ? null : this.back.clone();
            //        node.polygons = new ArrayList<>();
            //        polygons.parallelStream().forEach((Polygon p) -> {
            //            node.polygons.add(p.clone());
            //        });


            if (polygons.Count > 200)
            {
                node.polygons = polygons.AsParallel().Select(p => p.clone()).ToList();
            }
            else
            {
                node.polygons = polygons.Select(p => p.clone()).ToList();
            }

            

            return node;
        }

        /// <summary>
        /// Converts solid space to empty space and vice verca.
        /// </summary>
        public void invert()
        {


            if (polygons.Count > 200)
            {
                polygons.AsParallel().ForAll(polygon => polygon.flip());
            }
            else
            {
                polygons.ForEach(polygon => polygon.flip());
            }

            if (this.plane == null && polygons.Count != 0)
            {
                this.plane = polygons[0]._csg_plane.clone();
            }
            else if (this.plane == null && polygons.Count == 0)
            {

                Console.Error.WriteLine("Please fix me! I don't know what to do?");

                // throw new RuntimeException("Please fix me! I don't know what to do?");

                return;
            }

            this.plane.flip();

            if (this.front != null)
            {
                this.front.invert();
            }
            if (this.back != null)
            {
                this.back.invert();
            }
            Node temp = this.front;
            this.front = this.back;
            this.back = temp;
        }

        /// <summary>
        /// Recursively removes all polygons in the <see cref="polygons"/> list that are
        /// contained within this BSP tree.
        /// <b>Note:</b> polygons are splitted if necessary.
        /// </summary>
        /// 
        /// <param name="polygons">the polygons to clip</param>
        /// <returns>the cliped list of polygons</returns>
        /// 
        private List<Polygon> clipPolygons(List<Polygon> polygons)
        {

            if (this.plane == null)
            {
                return new List<Polygon>(polygons);
            }

            List<Polygon> frontP = new List<Polygon>();
            List<Polygon> backP = new List<Polygon>();

            foreach (Polygon polygon in polygons)
            {
                this.plane.splitPolygon(polygon, frontP, backP, frontP, backP);
            }
            if (this.front != null)
            {
                frontP = this.front.clipPolygons(frontP);
            }
            if (this.back != null)
            {
                backP = this.back.clipPolygons(backP);
            }
            else
            {
                backP = new List<Polygon>(0);
            }

            frontP.AddRange(backP);
            return frontP;
        }

        /// <summary>
        /// Removes all polygons in this BSP tree that are inside the specified BSP
        /// tree (<c>bsp</c>).
        /// <b>Note:</b> polygons are splitted if necessary.
        /// </summary>
        /// <param name="bsp">bsp that shall be used for clipping</param>
        /// 
        public void clipTo(Node bsp)
        {
            this.polygons = bsp.clipPolygons(this.polygons);
            if (this.front != null)
            {
                this.front.clipTo(bsp);
            }
            if (this.back != null)
            {
                this.back.clipTo(bsp);
            }
        }

        /// <summary>
        /// Returns a list of all polygons in this BSP tree.
        /// </summary>
        /// <returns>a list of all polygons in this BSP tree</returns>
        /// 
        public List<Polygon> allPolygons()
        {
            List<Polygon> localPolygons = new List<Polygon>(this.polygons);
            if (this.front != null)
            {
                localPolygons.AddRange(this.front.allPolygons());
                //            polygons = Utils.concat(polygons, this.front.allPolygons());
            }
            if (this.back != null)
            {
                //            polygons = Utils.concat(polygons, this.back.allPolygons());
                localPolygons.AddRange(this.back.allPolygons());
            }

            return localPolygons;
        }

        /// <summary>
        /// Build a BSP tree out of <c>polygons</c>. When called on an existing
        /// tree, the new polygons are filtered down to the bottom of the tree and
        /// become new nodes there. Each set of polygons is partitioned using the
        /// first polygon (no heuristic is used to pick a good split).
        /// </summary>
        /// <param name="polygons">polygons used to build the BSP</param>
        /// 
        public void build(List<Polygon> polygons)
        {

            if (polygons.Count == 0) return;

            if (this.plane == null)
            {
                this.plane = polygons[0]._csg_plane.clone();
            }

            polygons = polygons.Where(p=>p.isValid()).Distinct().ToList();

            List<Polygon> frontP = new List<Polygon>();
            List<Polygon> backP = new List<Polygon>();

            // parellel version does not work here
            polygons.ForEach((polygon)=> {
                this.plane.splitPolygon(
                        polygon, this.polygons, this.polygons, frontP, backP);
            });

            if (frontP.Count > 0)
            {
                if (this.front == null)
                {
                    this.front = new Node();
                }
                this.front.build(frontP);
            }
            if (backP.Count > 0)
            {
                if (this.back == null)
                {
                    this.back = new Node();
                }
                this.back.build(backP);
            }
        }
    }
}
