namespace logicpos.Classes.Logic.Others
{
    public class Position
    {
        private int _x;
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        private int _y;
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public Position() { }
        public Position(int pX, int pY)
        {
            _x = pX;
            _y = pY;
        }
    }
}
