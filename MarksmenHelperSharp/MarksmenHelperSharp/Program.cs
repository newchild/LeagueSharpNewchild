using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;

namespace MarksmenHelperSharp
{
	class Program
	{
		private static List<Geometry.Polygon> disabledPositions = new List<Geometry.Polygon>();
		private static PositionHelperMode mode = PositionHelperMode.Ultra;
		enum PositionHelperMode
		{
			Realistic,
			Ultra,
			AAOnly
		}
		private static Obj_AI_Hero Player
		{
			get
			{
				return ObjectManager.Player;
			}
		}
		static void Main(string[] args)
		{
			Game.OnStart+=Game_OnStart;
			Game.OnUpdate += Game_OnUpdate;
			Obj_AI_Hero.OnIssueOrder += Obj_AI_Hero_OnIssueOrder;
		}

		static void Obj_AI_Hero_OnIssueOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
		{
			if (!sender.IsMe)
				return;
			if (args.Order == GameObjectOrder.MoveTo || GameObjectOrder.AttackTo && mode == PositionHelperMode.Ultra)
			{
				foreach (var Polygon in disabledPositions)
				{
					if (Polygon.IsInside(args.TargetPosition))
					{
						args.Process = false;
					}
				}
			}
		}

		static void Game_OnUpdate(EventArgs args)
		{
			disabledPositions = getAllUnsafePositions();
			switch (mode)
			{
				case PositionHelperMode.Ultra:
					foreach (var Polygon in disabledPositions)
					{
						if(Player)
					}
			}
		}

		private static void getAllUnsafePositions()
		{
			List<Geometry.Polygon> dangerousPositions = new List<Geometry.Polygon>();
			foreach (var Hero in ObjectManager.Get<Obj_AI_Hero>())
			{
				var Q = Hero.Spellbook.GetSpell(SpellSlot.Q);
				var W = Hero.Spellbook.GetSpell(SpellSlot.W);
				var E = Hero.Spellbook.GetSpell(SpellSlot.E);
				var R = Hero.Spellbook.GetSpell(SpellSlot.R);
				var AARange = Hero.AttackRange;
				List<float> Ranges = new List<float>();
				if (Q.SData.CastRange <= 900 && Q.Cooldown <= 0)//filter global skillshots and Spells on cooldown
				{
					Ranges.Add(Q.SData.CastRange);
				}
				if (W.SData.CastRange <= 900 && W.Cooldown <= 0)//filter global skillshots and Spells on cooldown
				{
					Ranges.Add(W.SData.CastRange);
				}
				if (E.SData.CastRange <= 900 && E.Cooldown <= 0)//filter global skillshots and Spells on cooldown
				{
					Ranges.Add(E.SData.CastRange);
				}
				if (R.SData.CastRange <= 900 && R.Cooldown <= 0)//filter global skillshots and Spells on cooldown
				{
					Ranges.Add(R.SData.CastRange);
				}
				Ranges.Add(AARange);
				float maxRange = Ranges.Max();
				var Poly = new Geometry.Polygon();
				int sensitivity = 60;
				for(int i = 0; i < sensitivity; i ++){//Todo, allow user interactivity
					var Circle = new Vector2(Hero.Position + maxRange * Math.Cos(360/sensitivity), Hero.Position + maxRange * Math.Sin(360/sensitivity));
					Poly.Add(Circle);
				}
				dangerousPositions.Add(Poly);
			}
			return dangerousPositions;
		}

		static void Game_OnStart(EventArgs args)
		{
			Notification startup = new Notification("Marksmenhelper by newchild loaded");
		}
	}
}
