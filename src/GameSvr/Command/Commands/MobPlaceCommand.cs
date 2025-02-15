﻿using GameSvr.CommandSystem;
using SystemModule;

namespace GameSvr
{
    /// <summary>
    /// 设定怪物集中点
    /// </summary>
    [GameCommand("MobPlace", "设定怪物集中点", "X  Y 怪物名称 怪物数量", 10)]
    public class MobPlaceCommand : BaseCommond
    {
        [DefaultCommand]
        public void MobPlace(string[] @Params, TPlayObject PlayObject)
        {
            if (@Params == null)
            {
                return;
            }
            var sX = @Params.Length > 0 ? @Params[0] : "";
            var sY = @Params.Length > 1 ? @Params[1] : "";
            var sMonName = @Params.Length > 2 ? @Params[2] : "";
            var sCount = @Params.Length > 3 ? @Params[3] : "";
            var nCount = HUtil32._MIN(500, HUtil32.Str_ToInt(sCount, 0));
            var nX = (short)HUtil32.Str_ToInt(sX, 0);
            var nY = (short)HUtil32.Str_ToInt(sY, 0);
            Envirnoment MEnvir;
            TBaseObject mon = null;
            nCount = HUtil32._MIN(500, HUtil32.Str_ToInt(sCount, 0));
            nX = (short)HUtil32.Str_ToInt(sX, 0);
            nY = (short)HUtil32.Str_ToInt(sY, 0);
            if (nX <= 0 || nY <= 0 || sMonName == "" || nCount <= 0)
            {
                PlayObject.SysMsg(GameCommand.ShowHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            MEnvir = M2Share.MapManager.FindMap(M2Share.g_sMissionMap);
            if (!M2Share.g_boMission || MEnvir == null)
            {
                PlayObject.SysMsg("还没有设定怪物集中点!!!", MsgColor.Red, MsgType.Hint);
                PlayObject.SysMsg("请先用命令" + this.GameCommand.Name + "设置怪物的集中点。", MsgColor.Red, MsgType.Hint);
                return;
            }
            for (var i = 0; i < nCount; i++)
            {
                mon = M2Share.UserEngine.RegenMonsterByName(M2Share.g_sMissionMap, nX, nY, sMonName);
                if (mon != null)
                {
                    mon.m_boMission = true;
                    mon.m_nMissionX = M2Share.g_nMissionX;
                    mon.m_nMissionY = M2Share.g_nMissionY;
                }
                else
                {
                    break;
                }
            }
            if (mon?.m_btRaceServer != 136)
            {
                PlayObject.SysMsg(nCount + " 只 " + sMonName + " 已正在往地图 " + M2Share.g_sMissionMap + " " +
                    M2Share.g_nMissionX + ":" + M2Share.g_nMissionY + " 集中。", MsgColor.Green, MsgType.Hint);
            }
        }
    }
}