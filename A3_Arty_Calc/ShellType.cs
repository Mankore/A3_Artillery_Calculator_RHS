using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A3_Arty_Calc
{
    abstract public class ShellType
    {
        public abstract double airFriction { get; }
        public abstract double initSpeed { get; }
        public abstract string name { get; }
    }

    class _3BK13 : ShellType
    {
        public override double airFriction
        {
            get
            {
                return -0.0005;
            }
        }
        public override double initSpeed
        {
            get
            {
                return 726;
            }
        }
        public override string name
        {
            get
            {
                return "3BK13";
            }
        }
    }

    class _3OF56 : ShellType
    {
        public override double airFriction
        {
            get
            {
                return -0.00017;
            }
        }
        public override double initSpeed
        {
            get
            {
                return 690;
            }
        }
        public override string name
        {
            get
            {
                return "3OF56";
            }
        }
    }

    class _AMOS_HE : ShellType
    {
        public override double airFriction
        {
            get
            {
                return 0;
            }
        }
        public override double initSpeed
        {
            get
            {
                return 810;
            }
        }
        public override string name
        {
            get
            {
                return "HE_AMOS";
            }
        }
    }

    class _AMOS_LG : ShellType
    {
        public override double airFriction
        {
            get
            {
                return 0;
            }
        }
        public override double initSpeed
        {
            get
            {
                return 810;
            }
        }
        public override string name
        {
            get
            {
                return "HE_AMOS_LG";
            }
        }
    }

    class _AMOS_CLUSTER : ShellType
    {
        public override double airFriction
        {
            get
            {
                return 0;
            }
        }
        public override double initSpeed
        {
            get
            {
                return 810;
            }
        }
        public override string name
        {
            get
            {
                return "HE_AMOS_CLUSTER";
            }
        }
    }

    class _53_WOF_27 : ShellType
    {
        public override double airFriction
        {
            get
            {
                return 0;
            }
        }
        public override double initSpeed
        {
            get
            {
                return 655;
            }
        }
        public override string name
        {
            get
            {
                return "HE_53_WOF_27";
            }
        }
    }
}
