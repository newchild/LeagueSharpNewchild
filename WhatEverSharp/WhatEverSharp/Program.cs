using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace YorickSharp
{
	class Program
	{
#region definitions
		private static Menu MainMenu;
		private static Obj_AI_Hero Player
		{
			get
			{
				return ObjectManager.Player;
			}
		}

		private static string name = "Yorick";
		private static int ghoulCount = 0;
		private static Orbwalking.Orbwalker orbwalker;
		private static Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>()
		{
			{SpellSlot.Q, new Spell(SpellSlot.Q)},
			{SpellSlot.W, new Spell(SpellSlot.W, 600.0f, TargetSelector.DamageType.Magical)},
			{SpellSlot.E, new Spell(SpellSlot.E, 550.0f)},
			{SpellSlot.R, new Spell(SpellSlot.R, 850.0f)}
		};


#endregion
		static void Main(string[] args)
		{
			Game.OnGameLoad+=YorickSharpGame_OnGameLoad;
		}

		private static void YorickSharpGame_OnGameLoad(EventArgs args)
		{
			SpellData wSData = spells[SpellSlot.W].Instance.SData;
			spells[SpellSlot.W].SetSkillshot(wSData.SpellCastTime, wSData.LineWidth, wSData.MissileSpeed, false, SkillshotType.SkillshotCircle);
			MainMenu = new Menu("Yorick the Burritodigger by newchild", "Yorick#Main", true);
			//Set up the TS
			Menu targetSelector = new Menu("TargetSelector", "Yorick#Main.TS");
			TargetSelector.AddToMenu(targetSelector);
			MainMenu.AddSubMenu(targetSelector);
			//Set up the Orbwalker
			Menu orbwalkerMenu = new Menu("Orbwalker", "Yorick#Main.Orbwalker");
			orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
			MainMenu.AddSubMenu(orbwalkerMenu);
			Orbwalking.AfterAttack += YorickSharpAfterAttack;
			Game.OnUpdate += YorickSharpGame_OnUpdate;
		}

		private static void YorickSharpAfterAttack(AttackableUnit unit, AttackableUnit target)
		{
			if (unit.IsMe && spells[SpellSlot.Q].CanCast() && orbwalker.InAutoAttackRange(target))
			{
				spells[SpellSlot.Q].Cast();
				Orbwalking.ResetAutoAttackTimer();
			}
		}

		static void YorickSharpGame_OnUpdate(EventArgs args)
		{
			switch (orbwalker.ActiveMode)
			{
				case Orbwalking.OrbwalkingMode.Combo:
					YorickSharpCombo();
			}
			if (Player.HealthPercent <= 25)
			{
				if (spells[SpellSlot.R].CanCast(Player))
				{
					spells[SpellSlot.R].CastOnUnit(Player);
				}
			}
		}

		private static void YorickSharpCombo()
		{
			Obj_AI_Hero target = TargetSelector.GetTarget(600.0f, TargetSelector.DamageType.Physical); //select only 1 target to make it more legit
			if (spells[SpellSlot.W].CanCast(target))
			{
				spells[SpellSlot.W].Cast(target);
			}
			if (spells[SpellSlot.E].CanCast(target) )
			{
				spells[SpellSlot.E].CastOnUnit(target);
			}
		}
			
	}
}

