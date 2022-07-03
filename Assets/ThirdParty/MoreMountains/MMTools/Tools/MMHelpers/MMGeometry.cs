using System.Collections.Generic;

namespace MoreMountains.Tools
{
    /// <summary>
    ///     A helper class to handle geometry related operations
    /// </summary>
    public static class MMGeometry
    {
        public static List<MMEdge> GetEdges(int[] indices)
        {
            var edgeList = new List<MMEdge>();
            for (var i = 0; i < indices.Length; i += 3)
            {
                var vertice1 = indices[i];
                var vertice2 = indices[i + 1];
                var vertice3 = indices[i + 2];
                edgeList.Add(new MMEdge(vertice1, vertice2, i));
                edgeList.Add(new MMEdge(vertice2, vertice3, i));
                edgeList.Add(new MMEdge(vertice3, vertice1, i));
            }

            return edgeList;
        }

        public static List<MMEdge> FindBoundary(this List<MMEdge> edges)
        {
            var edgeList = new List<MMEdge>(edges);
            for (var i = edgeList.Count - 1; i > 0; i--)
            for (var n = i - 1; n >= 0; n--)
                // if we find a shared edge we remove both
                if (edgeList[i].Vertice1 == edgeList[n].Vertice2 && edgeList[i].Vertice2 == edgeList[n].Vertice1)
                {
                    edgeList.RemoveAt(i);
                    edgeList.RemoveAt(n);
                    i--;
                    break;
                }

            return edgeList;
        }

        public static List<MMEdge> SortEdges(this List<MMEdge> edges)
        {
            var edgeList = new List<MMEdge>(edges);
            for (var i = 0; i < edgeList.Count - 2; i++)
            {
                var E = edgeList[i];
                for (var n = i + 1; n < edgeList.Count; n++)
                {
                    var a = edgeList[n];
                    if (E.Vertice2 == a.Vertice1)
                    {
                        if (n == i + 1)
                        {
                            // if they're already in order, we move on
                            break;
                        }

                        // otherwise we swap
                        edgeList[n] = edgeList[i + 1];
                        edgeList[i + 1] = a;
                        break;
                    }
                }
            }

            return edgeList;
        }

        // Based on https://answers.unity.com/questions/1019436/get-outeredge-vertices-c.html
        public struct MMEdge
        {
            public int Vertice1;
            public int Vertice2;
            public int TriangleIndex;

            public MMEdge(int aV1, int aV2, int aIndex)
            {
                Vertice1 = aV1;
                Vertice2 = aV2;
                TriangleIndex = aIndex;
            }
        }
    }
}