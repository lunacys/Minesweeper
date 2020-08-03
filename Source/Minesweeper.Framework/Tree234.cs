using System;

namespace Minesweeper.Framework
{
    public class Tree234Node
    {
        public Tree234Node Parent { get; set; }
        public Tree234Node[] Kids { get; set; } = new Tree234Node[4];
        public int[] Counts { get; set; } = new int[4];
        public object[] Elements { get; set; } = new object[3];
    }
    
    public class Tree234
    {
        public Tree234Node Root { get; set; }
        public Action<object, object> Compare { get; set; }
        
        public Tree234(Action<object, object> compare)
        {
            Compare = compare;
        }
    }
}