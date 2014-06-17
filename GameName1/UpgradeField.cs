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

        private Weapon m_Weapon;
        public UpgradeField(Weapon weapon)
        {
            m_Weapon = weapon;
            Name = weapon.Name;
            m_Weapon.SetWeaponStats();
            WeaponStats tempStats = m_Weapon.GetWeaponStats();
            m_Value = tempStats.WeaponDamage;
        }
        public void Upgrade()
        {
            m_Weapon.UpgradeWeaponStats();
            m_Value = m_Weapon.GetWeaponStats().WeaponDamage;
        }
        public int GetUpgradeCost()
        {
            return m_Weapon.GetWeaponStats().NextUpgradeCost;
        }
        public int GetUpgradeLevel()
        {
            return m_Weapon.GetWeaponStats().WeaponLevel;
        }
    }
}
