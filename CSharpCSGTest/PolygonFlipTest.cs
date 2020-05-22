using CSharpCSG;
using CSharpVecMath;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CCSGTest
{
    [TestClass]
    public class PolygonFlipTest
    {
        private const double EPSILON = 1e-8;

        [TestMethod]
        public void FlipPolygonTest()
        {
            Polygon polygon = Polygon.fromPoints(
                    Vector3d.xy(1, 1),
                    Vector3d.xy(2, 1),
                    Vector3d.xy(1, 2)
            );
            assertEquals(Vector3d.z(1), polygon.getPlane().getNormal());
            polygon.flip();
            assertEquals(Vector3d.z(-1), polygon.getPlane().getNormal());
        }

        private void assertEquals(IVector3d expected, IVector3d actual)
        {
            Assert.AreEqual(expected.getX(), actual.getX(), EPSILON);
            Assert.AreEqual(expected.getY(), actual.getY(), EPSILON);
            Assert.AreEqual(expected.getZ(), actual.getZ(), EPSILON);
        }

    }
}
