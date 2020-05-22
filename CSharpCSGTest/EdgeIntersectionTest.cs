using CSharpCSG;
using CSharpVecMath;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CCSGTest
{
    /// 
    /// 
    /// @author Michael Hoffer &lt;info@michaelhoffer.de&gt;
    /// 
    [TestClass]
    public class EdgeIntersectionTest
    {

        [TestMethod]
        public void closestPointTest()
        {

            // closest point is e1p2
            createClosestPointTest(
                    Vector3d.xyz(1, 2, 3), /*e1p2*/ Vector3d.xyz(4, 5, 6),
                    Vector3d.xyz(4, 5, 7), Vector3d.xyz(0, 1, 7),
                    Vector3d.xyz(4, 5, 6));

            // parallel edges (result=null)
            createClosestPointTest(
                    Vector3d.xyz(1, 1, -1), Vector3d.xyz(1, 1, 1),
                    Vector3d.xyz(2, 2, -3), Vector3d.xyz(2, 2, 4),
                    null);
            createClosestPointTest(
                    Vector3d.xyz(1, 3, -1), Vector3d.xyz(1, 4, 2),
                    Vector3d.xyz(1 + 10, 3, -1), Vector3d.xyz(1 + 10, 4, 2),
                    null);
            createClosestPointTest(
                    Vector3d.xyz(3, 6, -1), Vector3d.xyz(10, 7, 1),
                    Vector3d.xyz(3, 6, -1 + 3), Vector3d.xyz(10, 7, 1 + 3),
                    null);

            // result is exactly in the middle of e1 and e2
            createClosestPointTest(
                    Vector3d.xyz(5, 4, 2), /*e1p2*/ Vector3d.xyz(3, 2, 11),
                    Vector3d.xyz(5, 2, 11), /*e1p2*/ Vector3d.xyz(3, 4, 2),
                    Vector3d.xyz(4, 3, 6.5));
        }

        [TestMethod]
        public void intersectionTest()
        {
            // closest point is e1p2 which does not exist on e2. thus, the expected
            // result is null
            createIntersectionTest(
                    Vector3d.xyz(1, 2, 3), /*e1p2*/ Vector3d.xyz(4, 5, 6),
                    Vector3d.xyz(4, 5, 7), Vector3d.xyz(0, 1, 7),
                    null);

            // parallel edges (result=null)
            createIntersectionTest(
                    Vector3d.xyz(1, 1, -1), Vector3d.xyz(1, 1, 1),
                    Vector3d.xyz(2, 2, -3), Vector3d.xyz(2, 2, 4),
                    null);
            createIntersectionTest(
                    Vector3d.xyz(1, 3, -1), Vector3d.xyz(1, 4, 2),
                    Vector3d.xyz(1 + 10, 3, -1), Vector3d.xyz(1 + 10, 4, 2),
                    null);
            createIntersectionTest(
                    Vector3d.xyz(3, 6, -1), Vector3d.xyz(10, 7, 1),
                    Vector3d.xyz(3, 6, -1 + 3), Vector3d.xyz(10, 7, 1 + 3),
                    null);

            // result is exactly in the middle of e1 and e2
            createIntersectionTest(
                    Vector3d.xyz(5, 4, 2), /*e1p2*/ Vector3d.xyz(3, 2, 11),
                    Vector3d.xyz(5, 2, 11), /*e1p2*/ Vector3d.xyz(3, 4, 2),
                    Vector3d.xyz(4, 3, 6.5));
        }

        private static void createIntersectionTest(
                IVector3d e1p1, IVector3d e1p2,
                IVector3d e2p1, IVector3d e2p2,
                IVector3d expectedPoint)
        {
            Edge e1 = new Edge(
                    new Vertex(
                            e1p1, Vector3d.Z_ONE),
                    new Vertex(
                            e1p2, Vector3d.Z_ONE));

            Edge e2 = new Edge(
                    new Vertex(
                            e2p1, Vector3d.Z_ONE),
                    new Vertex(
                            e2p2, Vector3d.Z_ONE));

            IVector3d closestPointResult = e1.getIntersection(e2);

            if (expectedPoint != null)
            {
                Assert.IsTrue(closestPointResult !=null, "Intersection point must exist");

                IVector3d closestPoint = closestPointResult;

                Assert.IsTrue(expectedPoint.Equals(closestPoint), "Intersection point " + expectedPoint + ", got "
                        + closestPoint);
            }
            else
            {
                Assert.IsFalse(closestPointResult != null, "Intersection point must not exist : "
                        + closestPointResult);
            }
        }

        private static void createClosestPointTest(
                IVector3d e1p1, IVector3d e1p2,
                IVector3d e2p1, IVector3d e2p2,
                IVector3d expectedPoint)
        {
            Edge e1 = new Edge(
                    new Vertex(
                            e1p1, Vector3d.Z_ONE),
                    new Vertex(
                            e1p2, Vector3d.Z_ONE));

            Edge e2 = new Edge(
                    new Vertex(
                            e2p1, Vector3d.Z_ONE),
                    new Vertex(
                            e2p2, Vector3d.Z_ONE));

            IVector3d closestPointResult = e1.getClosestPoint(e2);

            if (expectedPoint != null)
            {
                Assert.IsTrue(closestPointResult != null, "Closest point must exist");

                IVector3d closestPoint = closestPointResult;

                Assert.IsTrue(expectedPoint.Equals(closestPoint), "Expected point " + expectedPoint + ", got "
                        + closestPoint);
            }
            else
            {
                Assert.IsFalse(closestPointResult != null, "Closest point must not exist : "
                        + closestPointResult);
            }
        }

    }
}
