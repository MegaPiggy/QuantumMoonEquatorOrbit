using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace QuantumMoonEquatorOrbit
{
	public class QuantumMoonEquatorOrbit : ModBehaviour
	{
		public void Start()
		{
			new Harmony(ModHelper.Manifest.UniqueName).PatchAll();
		}
	}


	[HarmonyPatch]
	public class QuantumMoonEquatorOrbitPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(OWPhysics), nameof(OWPhysics.CalculateOrbitVelocity))]
		public static bool OWPhysics_CalculateOrbitVelocity_Prefix(OWRigidbody primaryBody, OWRigidbody satelliteBody, ref float orbitAngle, ref Vector3 __result)
		{
			if (satelliteBody == Locator.GetQuantumMoon().GetAttachedOWRigidbody())
			{
				orbitAngle = 0;
			}
			return true;
		}

		[HarmonyTranspiler]
		[HarmonyPatch(typeof(QuantumMoon), nameof(QuantumMoon.ChangeQuantumState))]
		public static IEnumerable<CodeInstruction> QuantumMoon_ChangeQuantumState_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			return new CodeMatcher(instructions, generator).MatchForward(false,
					new CodeMatch(OpCodes.Ldloc_2),
					new CodeMatch(OpCodes.Ldc_I4_5)
				).Advance(3).CreateLabel(out Label unitY).Advance(-3)
				.Insert(
					new CodeInstruction(OpCodes.Ldloc_2),
					new CodeInstruction(OpCodes.Ldc_I4_M1),
					new CodeInstruction(OpCodes.Bne_Un, unitY))
				.InstructionEnumeration();
		}
	}
}