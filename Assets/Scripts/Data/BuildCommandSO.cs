using System;
using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Build", menuName = "Scriptable Objects/Command Build")]
    public class BuildCommandSO : CommandSO
    {
        public TowerSO tower;
        public event Action<TowerSO> OnBuildCommand;

        public override bool TryCommand(string text)
        {
            // Tady jen ověříme, jestli text sedí, ale NIC nestavíme
            return base.TryCommand(text);
        }

        // Tuhle metodu zavoláme, až když budeme vědět, že máme peníze
        public void Execute()
        {
            OnBuildCommand?.Invoke(tower);
        }

        [ContextMenu("Build")]
        public void PlayBuild()
        {
            Execute();
        }
    }
}