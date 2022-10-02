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
        public abstract double thrust { get; }
        public abstract double thrustTime { get; }

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

        public override double thrust => throw new NotImplementedException();

        public override double thrustTime => throw new NotImplementedException();
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

        public override double thrust => throw new NotImplementedException();

        public override double thrustTime => throw new NotImplementedException();
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

        public override double thrust => throw new NotImplementedException();

        public override double thrustTime => throw new NotImplementedException();
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

        public override double thrust => throw new NotImplementedException();

        public override double thrustTime => throw new NotImplementedException();
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

        public override double thrust => throw new NotImplementedException();

        public override double thrustTime => throw new NotImplementedException();
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

        public override double thrust => throw new NotImplementedException();

        public override double thrustTime => throw new NotImplementedException();
    }

    class _S8 : ShellType
    {
        public override double airFriction
        {
            get
            {
                return -0.09;
            }
        }
        public override double initSpeed
        {
            get
            {
                return 44;
            }
        }
        public override string name
        {
            get
            {
                return "S8";
            }
        }

        public override double thrust
        {
            get
            {
                return 1060;
            }
        }

        public override double thrustTime
        {
            get
            {
                return 0.69;
            }
        }
    }

    class _M821_HE : ShellType
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
                return 200;
            }
        }
        public override string name
        {
            get
            {
                return "M821_HE";
            }
        }

        public override double thrust => throw new NotImplementedException();

        public override double thrustTime => throw new NotImplementedException();
    }

    class _3VO18 : ShellType
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
                return 211;
            }
        }
        public override string name
        {
            get
            {
                return "3VO18";
            }
        }

        public override double thrust => throw new NotImplementedException();

        public override double thrustTime => throw new NotImplementedException();
    }
}
