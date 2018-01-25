namespace logicpos.Classes.Logic.Others
{
    public class Position
    {
        int _x;
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        int _y;
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
