namespace ST
{
    public struct TargetData
    {
        public Ship Attacker;
        public Ship Target;
        public Salvo Salvo;
        public Side AttackingSide;
        public Side TargetSide;
        public Side? TargetWedge;
        public Side? TargetSideWall;
        public int Distance;
        public int Mql;
        public int Missiles;
    }
}