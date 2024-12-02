using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AreasNS {
    public class Areas {
        private List<Tuple<string, Polygon>> areas;

        public Areas() {
            this.areas = new List<Tuple<string, Polygon>>();
        }

        public void AddArea(string name, List<Tuple<double,double>> poly) {
            this.areas.Add(new Tuple<string, Polygon>(name, new Polygon(poly)));
        }

        public void AddArea(string name, List<Tuple<int, int>> poly) {
            var newPoly = new List<Tuple<double, double>>();
            foreach (var point in poly) {
                newPoly.Add(new Tuple<double, double>(point.Item1, point.Item2));
            }
            AddArea(name, newPoly);
        }

        public void AddArea(string name, List<(int, int)> poly) {
            AddArea(name, poly.Select(a => new Tuple<int, int>(a.Item1, a.Item2)).ToList());
        }

        internal string BestArea(Point point) {
            string bestArea = "";
            double bestAreaSize = 0;
            foreach ((var name, var poly) in this.areas) {
                double nothing = poly.Area();
                if (poly.Contains(point)) {
                    if (bestArea == "" ||
                        poly.Area() < bestAreaSize) {
                        bestArea = name;
                        bestAreaSize = poly.Area();
                    }
                }
            }
            return bestArea;
        }

        public string BestArea(double x, double y) {
            return BestArea(new Point(x, y));
        }

        internal List<string> AllAreasWithPoint(Point point) {
            List<string> areasWithPoint = new List<string>();
            foreach ((var name, var poly) in this.areas) {
                if (poly.Contains(point)) {
                    areasWithPoint.Add(name);
                }
            }
            return areasWithPoint.Distinct().ToList();
        }

        public List<string> AllAreasWithPoint(double x, double y) {
            return AllAreasWithPoint(new Point(x, y));
        }
    }

    internal class Point {
        private double x, y;

        internal Point(double x, double y) {
            this.x = x;
            this.y = y;
        }

        internal double GetX() { return x; }

        internal double GetY() { return y; }

        public override string ToString() {
            return $"({this.x}, {this.y})";
        }
    }

    internal class Line {
        private Point start, end;

        internal Line(Point start, Point end) {
            this.start = start;
            this.end = end;
        }

        internal Line(double x1, double y1, double x2, double y2) {
            this.start = new Point(x1, y1);
            this.end = new Point(x2, y2);
        }

        internal Point getStart() { return start; }
        public Point getEnd() { return end; }

        internal Point? Intersection(Line line) {
            Point A = this.start;
            Point B = this.end;
            Point C = line.getStart();
            Point D = line.getEnd();

            // Line AB represented as a1x + b1y = c1
            double a1 = B.GetY() - A.GetY();
            double b1 = A.GetX() - B.GetX();
            double c1 = a1 * A.GetX() + b1 * A.GetY();

            // Line CD represented as a2x + b2y = c2 
            double a2 = D.GetY() - C.GetY();
            double b2 = C.GetX() - D.GetX();
            double c2 = a2 * C.GetX() + b2 * C.GetY();

            double determinant = a1 * b2 - a2 * b1;

            if (determinant < 0.0001 & determinant > -0.0001) {
                // parallel lines
                return null;
            }

            Point testPoint = new Point((b2 * c1 - b1 * c2) / determinant, (a1 * c2 - a2 * c1) / determinant);

            if (testPoint.GetX() > Math.Max(this.start.GetX(), this.end.GetX()) ||
                testPoint.GetX() > Math.Max(line.start.GetX(), line.end.GetX()) ||
                testPoint.GetX() < Math.Min(this.start.GetX(), this.end.GetX()) ||
                testPoint.GetX() < Math.Min(line.start.GetX(), line.end.GetX()) ||
                testPoint.GetY() > Math.Max(this.start.GetY(), this.end.GetY()) ||
                testPoint.GetY() > Math.Max(line.start.GetY(), line.end.GetY()) ||
                testPoint.GetY() < Math.Min(this.start.GetY(), this.end.GetY()) ||
                testPoint.GetY() < Math.Min(line.start.GetY(), line.end.GetY())) {
                // Intersection outside of either line segment
                return null;
            } else {
                return testPoint;
            }
        }

        internal bool Intersects(Line line) {
            return Intersection(line) != null;
        }

        internal int CountIntersects(List<Line> lines) {
            int count = 0;
            foreach (Line line in lines) {
                if (this.Intersects(line)) { count++; }
            }
            return count;
        }

        public override string ToString() {
            return start + "<->" + end;
        }
    }

    internal class Polygon {
        private List<Point> points;

        internal Polygon(List<Point> points) {
            if (points.Count < 3) {
                throw new ArgumentException("Polygon must consist of at least 3 points");
            }
            this.points = new List<Point>(points);
        }

        internal Polygon(Polygon poly) {
            this.points = new List<Point>(poly.points);
        }

        internal Polygon (List<Tuple<double, double>> points) {
            if (points.Count < 3) {
                throw new ArgumentException("Polygon must consist of at least 3 points");
            }
            this.points = new List<Point>();
            foreach (var (x, y) in points) {
                this.points.Add(new Point(x, y));
            }
        }

        internal double Area() {
            double partial = 0;
            for (int cur = 0; cur < points.Count - 1; cur++) {
                int nex = cur + 1;
                Point curr = points[cur];
                Point next = points[nex];

                partial += (curr.GetX() * next.GetY()) - (curr.GetY() * next.GetX());
            }

            partial += points[points.Count - 1].GetX() * points[0].GetY() - points[points.Count - 1].GetY() * points[0].GetX();

            return Math.Abs(partial / 2.0);
        }

        internal List<Line> AllLines() {
            List<Line> allLines = new List<Line>();
            for (int i = 0; i < points.Count - 1; i++) {
                allLines.Add(new Line(points[i], points[i + 1]));
            }
            allLines.Add(new Line(points[points.Count - 1], points[0]));
            return allLines;
        }

        internal double GetMaxX() {
            double rightMost = points[0].GetX();
            foreach (var point in points) {
                if (point.GetX() > rightMost) { rightMost = point.GetX(); }
            }
            return rightMost;
        }

        internal double GetMinX() {
            double leftMost = points[0].GetX();
            foreach (var point in points) {
                if (point.GetX() < leftMost) { leftMost = point.GetX(); }
            }
            return leftMost;
        }

        internal double GetMaxY() {
            double topMost = points[0].GetX();
            foreach (var point in points) {
                if (point.GetY() > topMost) { topMost = point.GetY(); }
            }
            return topMost;
        }

        internal double GetMinY() {
            double bottomMost = points[0].GetX();
            foreach (var point in points) {
                if (point.GetY() < bottomMost) { bottomMost = point.GetY(); }
            }
            return bottomMost;
        }

        internal bool Contains(Point point) {
            Line offRight = new Line(point, new Point(GetMaxX() + 50, point.GetY()));
            int intersectCount = offRight.CountIntersects(this.AllLines());
            return intersectCount % 2 == 1;
        }

        internal bool Contains(double x, double y) {
            return Contains(new Point(x, y));
        }

        public override string ToString() {
            return "[" + string.Join(", ", points) + "]";
        }
    }
}
