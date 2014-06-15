using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameName1
{
    public class UpgradeField
    {
        private int m_Value;
        public int Value { get { return m_Value; } set { m_Value = value; } }
        public string Description;
        private int UpgradeAmount;
        public int UpgradeLevel;
        private int NextUpgradeCost;

        public UpgradeField(string desc)
        {
            Description = desc;
        }
        public void Upgrade()
        {
            m_Value += UpgradeAmount;
        }
        public int GetUpgradeCost()
        {
            return NextUpgradeCost;
        }
    }
}
