using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FourFrame.TopDown
{
    public class GridMap : MonoBehaviour
    {
        #region SINGLETON
        public static GridMap Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                if (Instance != this)
                {
                    Destroy(gameObject);
                }
            }

        }


        #endregion SINGLETON

        public Transform origin;
        public float unit;

        public LayerMask collLayer;
        public LayerMask itemLayer;

        public Vector2 Point2World(Point _pos)
        {
            var originPoint = origin.position;
            var newX = originPoint.x + _pos.x * unit;
            var newY = originPoint.y + _pos.y * unit;
            return new Vector2(newX, newY);
        }

        public Point World2Point(Vector2 _worldPos)
        {
            var originPoint = origin.position;
            var newX = (int) ((_worldPos.x - originPoint.x) / unit);
            var newY = (int) ((_worldPos.y - originPoint.y) / unit);
            return new Point(newX, newY);
        }

    }

    [System.Serializable]
    public class Point
    {
        public int x;
        public int y;

        public Point()
        {
            x = 0;
            y = 0;
        }


        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator +(Point p1, Point p2)
        {
            Point p = new Point();
            p.x = p1.x + p2.x;
            p.y = p1.y + p2.y;
            return p;
        }



        public static Point operator -(Point p1, Point p2)
        {
            Point p = new Point();
            p.x = p1.x - p2.x;
            p.y = p1.y - p2.y;
            return p;
        }

        /// <summary>
        /// Format: ({x}, {y})
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }
    }


}


