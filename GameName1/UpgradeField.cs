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
        public string Name;
        public int UpgradeLevel;

        private Weapon m_Weapon;
        private WeaponStats m_WeaponStats;
        public UpgradeField(Weapon weapon)
        {
            Name = weapon.Name;
            UpgradeLevel = 0;
            m_Weapon = weapon;
            m_WeaponStats = m_Weapon.GetWeaponStats(UpgradeLevel);
            m_Value = m_WeaponStats.WeaponDamage;
        }
        public void Upgrade()
        {
            ++UpgradeLevel;
            m_WeaponStats = m_Weapon.GetWeaponStats(UpgradeLevel);
        }
        public int GetUpgradeCost()
        {
            return m_WeaponStats.NextUpgradeCost;
        }
    }
}
