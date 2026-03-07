using System;
using UnityEngine;

namespace Dubinci
{
    [CreateAssetMenu(fileName = "Modifier", menuName = "Scriptable Objects/Command Modifier")]
    public class BuildModifierSO : CommandSO
    {
        public ModifierCellSO modifier;
        public event Action<ModifierCellSO> OnBuildCommand;

        public override bool TryCommand(string text)
        {
            // Tady jen ověříme, jestli text sedí, ale NIC nestavíme
            return base.TryCommand(text);
        }

        // Tuhle metodu zavoláme, až když budeme vědět, že máme peníze
        public void Execute()
        {
            OnBuildCommand?.Invoke(modifier);
        }

        [ContextMenu("Build")]
        public void PlayBuild()
        {
            Execute();
        }
    }
}