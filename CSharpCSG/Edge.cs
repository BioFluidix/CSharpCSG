/*
 * Edge.cs
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
    /// 
    /// 
    /// author: miho
    /// 
    public class Edge : IEquatable<Edge>
    {

        private readonly Vertex p1;
        private readonly Vertex p2;
        private readonly IVector3d direction;

        public Edge(Vertex p1, Vertex p2)
        {
            this.p1 = p1;
            this.p2 = p2;

            direction = p2.pos.minus(p1.pos).normalized();
        }

        ///
        /// <returns>the p1</returns>
        ///
        public Vertex getP1()
        {
            return p1;
        }

        //    /**
        //     * <param name="p1">the p1 to set</param>
        //     */
        //    public void setP1(Vertex p1) {
        //        this.p1 = p1;
        //    }
        ///
        /// <returns>the p2</returns>
        ///
        public Vertex getP2()
        {
            return p2;
        }

        //    /**
        //     * <param name="p2">the p2 to set</param>
        //     */
        //    public void setP2(Vertex p2) {
        //        this.p2 = p2;
        //    }
        public static List<Edge> fromPolygon(Polygon poly)
        {
            List<Edge> result = new List<Edge>();

            for (int i = 0; i < poly.vertices.Count; i++)
            {
                Edge e = new Edge(poly.vertices[i], poly.vertices[(i + 1) % poly.vertices.Count]);

                result.Add(e);
            }

            return result;
        }

        public static List<Vertex> toVertices(List<Edge> edges)
        {
            return edges.Select(e => e.p1).ToList();
        }

        public static List<IVector3d> toPoints(List<Edge> edges)
        {
            return edges.Select(e => e.p1.pos).ToList();
        }

        private static Polygon toPolygon(List<IVector3d> points, Plane plane)
        {

            //        List<Vector3d> points = edges.().Select(e => e.p1.pos).
            //                .ToList();
            Polygon p = Polygon.fromPoints(points);

            p.vertices.ForEach(vertex => vertex.normal = plane.normal.clone());

            // TODO Find out the meaning of foEachOrdered
            //p.vertices.forEachOrdered(vertex => {
            //    vertex.normal = plane.normal.clone();
            //});


            //        // we try to detect wrong orientation by comparing normals
            //        if (p.plane.normal.angle(plane.normal) > 0.1) {
            //            p.flip();
            //        }
            return p;
        }

        public static List<Polygon> toPolygons(List<Edge> boundaryEdges, Plane plane)
        {

            List<IVector3d> boundaryPath = new List<IVector3d>();

            bool[] used = new bool[boundaryEdges.Count];
            Edge edge = boundaryEdges[0];
            used[0] = true;
            while (true)
            {
                Edge finalEdge = edge;

                boundaryPath.Add(finalEdge.p1.pos);

                int nextEdgeIndex = boundaryEdges.IndexOf(boundaryEdges.
                        Where(e => finalEdge.p2.Equals(e.p1)).First());

                if (used[nextEdgeIndex])
                {
                    //                Console.Out.WriteLine("nexIndex: " + nextEdgeIndex);
                    break;
                }
                //            Console.Out.Write("edge: " + edge.p2.pos);
                edge = boundaryEdges[nextEdgeIndex];
                //            Console.Out.WriteLine("=> edge: " + edge.p1.pos);
                used[nextEdgeIndex] = true;
            }

            List<Polygon> result = new List<Polygon>();

            Console.Out.WriteLine("#bnd-path-length: " + boundaryPath.Count);

            result.Add(toPolygon(boundaryPath, plane));

            return result;
        }

        //private class Node<T>
        //{

        //    private Node parent;
        //    private readonly List<Node> children = new List<Node>();
        //    private readonly int index;
        //    private readonly T value;
        //private bool isHole;

        //    public Node(int index, T value)
        //    {
        //        this.index = index;
        //        this.value = value;
        //    }

        //    public void addChild(int index, T value)
        //    {
        //        children.Add(new Node(index, value));
        //    }

        //    public List<Node> getChildren()
        //    {
        //        return this.children;
        //    }

        //    /**
        //     * <returns>the parent</returns>
        //     */
        //    public Node getParent()
        //    {
        //        return parent;
        //    }

        //    /**
        //     * <returns>the index</returns>
        //     */
        //    public int getIndex()
        //    {
        //        return index;
        //    }

        //    /**
        //     * <returns>the value</returns>
        //     */
        //    public T getValue()
        //    {
        //        return value;
        //    }

        //    @Override
        //    public int hashCode()
        //    {
        //        int hash = 7;
        //        hash = 67 * hash + this.index;
        //        return hash;
        //    }

        //    @Override
        //    public bool Equals(Object obj)
        //    {
        //        if (obj == null)
        //        {
        //            return false;
        //        }
        //        if (getClass() != obj.getClass())
        //        {
        //            return false;
        //        }
        //        final Node<?> other = (Node <?>) obj;
        //        if (this.index != other.index)
        //        {
        //            return false;
        //        }
        //        return true;
        //    }

        //    public int distanceToRoot()
        //    {
        //        int dist = 0;

        //        Node pNode = getParent();

        //        while (pNode != null)
        //        {
        //            dist++;
        //            pNode = getParent();
        //        }

        //        return dist;
        //    }

        //    /**
        //     * <returns>the isHole</returns>
        //     */
        //    public bool isIsHole()
        //    {
        //        return isHole;
        //    }

        //    /**
        //     * <param name="isHole">the isHole to set</param>
        //     */
        //    public void setIsHole(bool isHole)
        //    {
        //        this.isHole = isHole;
        //    }

        //}

        public const string KEY_POLYGON_HOLES = "jcsg:edge:polygon-holes";

        public static List<Polygon> boundaryPathsWithHoles(List<Polygon> boundaryPaths)
        {

            List<Polygon> result = boundaryPaths.Select(p => p.clone()).ToList();

            List<List<int>> parents = new List<List<int>>();
            bool[] isHole = new bool[result.Count];

            for (int i = 0; i < result.Count; i++)
            {
                Polygon p1 = result[i];
                List<int> parentsOfI = new List<int>();
                parents.Add(parentsOfI);
                for (int j = 0; j < result.Count; j++)
                {
                    Polygon p2 = result[j];
                    if (i != j)
                    {
                        if (p2.contains(p1))
                        {
                            parentsOfI.Add(j);
                        }
                    }
                }
                isHole[i] = parentsOfI.Count % 2 != 0;
            }

            int[] parent = new int[result.Count];

            for (int i = 0; i < parent.Length; i++)
            {
                parent[i] = -1;
            }

            for (int i = 0; i < parents.Count; i++)
            {
                List<int> par = parents[i];

                int max = 0;
                int maxIndex = 0;
                foreach (int pIndex in par)
                {

                    int pSize = parents[pIndex].Count;

                    if (max < pSize)
                    {
                        max = pSize;
                        maxIndex = pIndex;
                    }
                }

                parent[i] = maxIndex;

                if (!isHole[maxIndex] && isHole[i])
                {

                    List<Polygon> holes;

                    List<Polygon> holesOpt = result[maxIndex].
                            getStorage().getValue<List<Polygon>>(KEY_POLYGON_HOLES);

                    if (holesOpt != null)
                    {
                        holes = holesOpt;
                    }
                    else
                    {
                        holes = new List<Polygon>();
                        result[maxIndex].getStorage().
                                set(KEY_POLYGON_HOLES, holes);
                    }

                    holes.Add(result[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a list of all boundary paths.
        /// </summary>
        /// <param name="boundaryEdges">boundary edges (all paths must be closed)</param>
        /// <returns></returns>
        private static List<Polygon> boundaryPaths(List<Edge> boundaryEdges)
        {
            List<Polygon> result = new List<Polygon>();

            bool[] used = new bool[boundaryEdges.Count];
            int startIndex = 0;
            Edge edge = boundaryEdges[startIndex];
            used[startIndex] = true;

            startIndex = 1;

            while (startIndex > 0)
            {
                List<IVector3d> boundaryPath = new List<IVector3d>();

                while (true)
                {
                    Edge finalEdge = edge;

                    boundaryPath.Add(finalEdge.p1.pos);

                    Console.Out.Write("edge: " + edge.p2.pos);

                    Edge nextEdgeResult = boundaryEdges.
                            Where(e => finalEdge.p2.Equals(e.p1)).First();

                    if (nextEdgeResult == null)
                    {
                        Console.Out.WriteLine("ERROR: unclosed path:"
                                + " no edge found with " + finalEdge.p2);
                        break;
                    }

                    Edge nextEdge = nextEdgeResult;

                    int nextEdgeIndex = boundaryEdges.IndexOf(nextEdge);

                    if (used[nextEdgeIndex])
                    {
                        break;
                    }

                    edge = nextEdge;
                    Console.Out.WriteLine("=> edge: " + edge.p1.pos);
                    used[nextEdgeIndex] = true;
                }

                if (boundaryPath.Count < 3)
                {
                    break;
                }

                result.Add(Polygon.fromPoints(boundaryPath));
                startIndex = nextUnused(used);

                if (startIndex > 0)
                {
                    edge = boundaryEdges[startIndex];
                    used[startIndex] = true;
                }

            }

            Console.Out.WriteLine("paths: " + result.Count);

            return result;
        }

        /// <summary>
        /// Returns the next unused index as specified in the given bool array.
        /// </summary>
        /// <param name="usage">the usage array</param>
        /// <returns>the next unused index or a value &lt; 0 if all indices are used</returns>
        /// 
        private static int nextUnused(bool[] usage)
        {
            for (int i = 0; i < usage.Length; i++)
            {
                if (usage[i] == false)
                {
                    return i;
                }
            }

            return -1;
        }

        public static List<Polygon> _toPolygons(List<Edge> boundaryEdges, Plane plane)
        {

            List<IVector3d> boundaryPath = new List<IVector3d>();

            bool[] used = new bool[boundaryEdges.Count];
            Edge edge = boundaryEdges[0];
            used[0] = true;
            while (true)
            {
                Edge finalEdge = edge;

                boundaryPath.Add(finalEdge.p1.pos);

                int nextEdgeIndex = boundaryEdges.IndexOf(boundaryEdges.
                        Where(e => finalEdge.p2.Equals(e.p1)).First());

                if (used[nextEdgeIndex])
                {
                    //                Console.Out.WriteLine("nexIndex: " + nextEdgeIndex);
                    break;
                }
                //            Console.Out.Write("edge: " + edge.p2.pos);
                edge = boundaryEdges[nextEdgeIndex];
                //            Console.Out.WriteLine("=> edge: " + edge.p1.pos);
                used[nextEdgeIndex] = true;
            }

            List<Polygon> result = new List<Polygon>();

            Console.Out.WriteLine("#bnd-path-length: " + boundaryPath.Count);

            result.Add(toPolygon(boundaryPath, plane));

            return result;
        }

        /// <summary>
        /// Determines whether the specified point lies on tthis edge.
        /// </summary>
        /// <param name="p">point to check</param>
        /// <param name="TOL">tolerance</param>
        /// <returns><code>true</code> if the specified point lies on this line
        /// segment; <code>false</code> otherwise
        /// 
        public bool contains(IVector3d p, double TOL)
        {

            double x = p.x();
            double x1 = this.p1.pos.x();
            double x2 = this.p2.pos.x();

            double y = p.y();
            double y1 = this.p1.pos.y();
            double y2 = this.p2.pos.y();

            double z = p.z();
            double z1 = this.p1.pos.z();
            double z2 = this.p2.pos.z();

            double AB = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
            double AP = Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1) + (z - z1) * (z - z1));
            double PB = Math.Sqrt((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y) + (z2 - z) * (z2 - z));

            return Math.Abs(AB - (AP + PB)) < TOL;
        }

        /// <summary>
        /// Determines whether the specified point lies on tthis edge.
        /// </summary>
        /// <param name="p">point to check</param>
        /// <returns><code>true</code> if the specified point lies on this line
        /// segment; <code>false</code> otherwise</returns>
        /// 
        public bool contains(IVector3d p)
        {
            return contains(p, Plane.EPSILON);
        }

        public override int GetHashCode()
        {
            int hash = 7;
            hash = 71 * hash + p1.GetHashCode();
            hash = 71 * hash + p2.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            Edge e = obj as Edge;
            return (e == null) ? false : Equals(e);
        }

        public bool Equals(Edge e)
        {
            if (!(p1.Equals(e.p1) || p2.Equals(e.p1)))
            {
                return false;
            }
            if (!(p2.Equals(e.p2) || p1.Equals(e.p2)))
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(Edge e0, Edge e1)
        {
            if (ReferenceEquals(e0, e1)) return true;
            if (ReferenceEquals(e0, null)) return false;
            if (ReferenceEquals(e1, null)) return false;

            return e0.Equals(e1);
        }

        public static bool operator !=(Edge e0, Edge e1)
        {
            if (ReferenceEquals(e0, e1)) return false;
            if (ReferenceEquals(e0, null)) return true;
            if (ReferenceEquals(e1, null)) return true;

            return !e0.Equals(e1);
        }


        public IVector3d getDirection()
        {
            return direction;
        }

        /// <summary>
        /// Returns the the point of this edge that is closest to the specified edge.
        /// 
        /// <b>NOTE:</b> returns an empty optional if the edges are parallel
        /// </summary>
        /// <param name="e">the edge to check</param>
        /// <returns>the the point of this edge that is closest to the specified edge</returns>
        /// 
        public IVector3d getClosestPoint(Edge e)
        {

            // algorithm from:
            // org.apache.commons.math3.geometry.euclidean.threed/Line.java.html
            IVector3d ourDir = getDirection();

            double cos = ourDir.dot(e.getDirection());
            double n = 1 - cos * cos;

            if (n < Plane.EPSILON)
            {
                // the lines are parallel
                return null;
            }

            IVector3d thisDelta = p2.pos.minus(p1.pos);
            double norm2This = thisDelta.magnitudeSq();

            IVector3d eDelta = e.p2.pos.minus(e.p1.pos);
            double norm2E = eDelta.magnitudeSq();

            // line points above the origin
            IVector3d thisZero = p1.pos.plus(thisDelta.times(-p1.pos.dot(thisDelta) / norm2This));
            IVector3d eZero = e.p1.pos.plus(eDelta.times(-e.p1.pos.dot(eDelta) / norm2E));

            IVector3d delta0 = eZero.minus(thisZero);
            double a = delta0.dot(direction);
            double b = delta0.dot(e.direction);

            IVector3d closestP = thisZero.plus(direction.times((a - b * cos) / n));

            if (!contains(closestP))
            {
                if (closestP.minus(p1.pos).magnitudeSq()
                        < closestP.minus(p2.pos).magnitudeSq())
                {
                    return p1.pos;
                }
                else
                {
                    return p2.pos;
                }
            }

            return closestP;
        }

        /// <summary>
        /// Returns the intersection point between this edge and the specified edge.
        /// </summary>
        /// <b>NOTE:</b> returns an empty optional if the edges are parallel or if
        /// the intersection point is not inside the specified edge segment
        /// 
        /// <param name="e">edge to intersect</param>
        /// <returns>the intersection point between this edge and the specified edge</returns>
        /// 
        public IVector3d getIntersection(Edge e)
        {
            IVector3d closestP = getClosestPoint(e);

            if (closestP == null)
            {
                // edges are parallel
                return null;
            }


            if (e.contains(closestP))
            {
                return closestP;
            }
            else
            {
                // intersection point outside of segment
                return null;
            }
        }

        public static List<Polygon> boundaryPolygons(CSG csg)
        {
            List<Polygon> result = new List<Polygon>();

            foreach (List<Polygon> polygonGroup in searchPlaneGroups(csg.getPolygons()))
            {
                result.AddRange(boundaryPolygonsOfPlaneGroup(polygonGroup));
            }

            return result;
        }

        private static List<Edge> boundaryEdgesOfPlaneGroup(List<Polygon> planeGroup)
        {
            List<Edge> edges = new List<Edge>();

            //Stream<Polygon> pStream;

            // if (planeGroup.Count > 200) {
            //     pStream = planeGroup.parallelStream();
            // } else {
            //pStream = planeGroup;
            // }

            planeGroup.Select(p => Edge.fromPolygon(p)).ToList().ForEach(pEdges =>
            {
                edges.AddRange(pEdges);
            });

            //Stream<Edge> edgeStream;

            // if (edges.Count > 200) {
            // edgeStream = edges.parallelStream();
            // } else {
            //edgeStream = edges;
            // }

            // find potential boundary edges, i.e., edges that occur once (freq=1)
            List<Edge> potentialBoundaryEdges = new List<Edge>();
            edges.ForEach(e =>
            {
                int count = edges.Where(edg => edg.Equals(e)).Count(); //  Collections.frequency(edges, e);
                if (count == 1)
                {
                    potentialBoundaryEdges.Add(e);
                }
            });

            // TODO figure meaning of forEachOrdered in this context
            //edges.forEachOrdered((e) => {
            //    int count = Collections.frequency(edges, e);
            //    if (count == 1)
            //    {
            //        potentialBoundaryEdges.add(e);
            //    }
            //});



            // now find "false boundary" edges end remove them from the 
            // boundary-edge-list
            // 
            // thanks to Susanne Höllbacher for the idea :)
            //Stream<Edge> bndEdgeStream;
            List<Edge> realBndEdges;
            if (potentialBoundaryEdges.Count > 200)
            {
                realBndEdges = potentialBoundaryEdges.AsParallel().Where(be => edges.Where(
                                    e => falseBoundaryEdgeSharedWithOtherEdge(be, e)
                            ).Count() == 0).ToList();

                //bndEdgeStream = potentialBoundaryEdges.parallelStream();
            }
            else
            {
                realBndEdges = potentialBoundaryEdges.
                    Where(be => edges.Where(
                                    e => falseBoundaryEdgeSharedWithOtherEdge(be, e)
                            ).Count() == 0).ToList();
                //bndEdgeStream = potentialBoundaryEdges;
            }



            //
            //        Console.Out.WriteLine("#bnd-edges: " + realBndEdges.Count
            //                + ",#edges: " + edges.Count
            //                + ", #del-bnd-edges: " + (boundaryEdges.Count - realBndEdges.Count));
            return realBndEdges;
        }

        private static List<Polygon> boundaryPolygonsOfPlaneGroup(
                List<Polygon> planeGroup)
        {

            List<Polygon> polygons = boundaryPathsWithHoles(
                    boundaryPaths(boundaryEdgesOfPlaneGroup(planeGroup)));

            Console.Out.WriteLine("polygons: " + polygons.Count);

            List<Polygon> result = new List<Polygon>(polygons.Count);

            foreach (Polygon p in polygons)
            {

                List<Polygon> holesOfPresult
                        = p.getStorage().getValue<List<Polygon>>(Edge.KEY_POLYGON_HOLES);

                if (holesOfPresult == null)
                {
                    result.Add(p);
                }
                else
                {
                    result.AddRange(PolygonUtil.concaveToConvex(p));
                }
            }

            return result;
        }

        private static bool falseBoundaryEdgeSharedWithOtherEdge(Edge fbe, Edge e)
        {

            // we don't consider edges with shared end-points since we are only
            // interested in "false-boundary-edge"-cases
            bool sharedEndPoints = e.getP1().pos.Equals(fbe.getP1().pos)
                    || e.getP1().pos.Equals(fbe.getP2().pos)
                    || e.getP2().pos.Equals(fbe.getP1().pos)
                    || e.getP2().pos.Equals(fbe.getP2().pos);

            if (sharedEndPoints)
            {
                return false;
            }

            return fbe.contains(e.getP1().pos) || fbe.contains(e.getP2().pos);
        }

        private static List<List<Polygon>> searchPlaneGroups(List<Polygon> polygons)
        {
            List<List<Polygon>> planeGroups = new List<List<Polygon>>();
            bool[] used = new bool[polygons.Count];
            Console.Out.WriteLine("#polys: " + polygons.Count);
            for (int pOuterI = 0; pOuterI < polygons.Count; pOuterI++)
            {

                if (used[pOuterI])
                {
                    continue;
                }

                Polygon pOuter = polygons[pOuterI];

                List<Polygon> otherPolysInPlane = new List<Polygon>();

                otherPolysInPlane.Add(pOuter);

                for (int pInnerI = 0; pInnerI < polygons.Count; pInnerI++)
                {

                    Polygon pInner = polygons[pInnerI];

                    if (pOuter.Equals(pInner))
                    {
                        continue;
                    }

                    IVector3d nOuter = pOuter._csg_plane.normal;
                    IVector3d nInner = pInner._csg_plane.normal;

                    // TODO do we need radians or degrees?
                    double angle = nOuter.angle(nInner);

                    //                Console.Out.WriteLine("angle: " + angle + " between " + pOuterI+" => " + pInnerI);
                    if (angle < 0.01 /*&& abs(pOuter.plane.dist - pInner.plane.dist) < 0.1*/)
                    {
                        otherPolysInPlane.Add(pInner);
                        used[pInnerI] = true;
                        Console.Out.WriteLine("used: " + pOuterI + " => " + pInnerI);
                    }
                }

                if (otherPolysInPlane.Count != 0)
                {
                    planeGroups.Add(otherPolysInPlane);
                }
            }
            return planeGroups;
        }

    }
}
