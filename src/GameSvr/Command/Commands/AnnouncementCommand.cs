﻿using GameSvr.CommandSystem;

namespace GameSvr
{
    /// <summary>
    /// 查看行会争霸赛结果
    /// </summary>
    [GameCommand("Announcement", "查看行会争霸赛结果", 10)]
    public class AnnouncementCommand : BaseCommond
    {
        [DefaultCommand]
        public void Announcement(string[] @Params, TPlayObject PlayObject)
        {
            //string sGuildName = @Params.Length > 0 ? @Params[0] : "";
            //TGUild Guild;
            //string sHumanName;
            //int nPoint;
            //if ((sGuildName == "") || ((sGuildName != "") && (sGuildName[0] == '?')))
            //{
            //    PlayObject.SysMsg("查看行会争霸赛结果。", TMsgColor.c_Red, TMsgType.t_Hint);
            //    PlayObject.SysMsg(string.Format("命令格式: @{0} 行会名称", this.Attributes.Name), TMsgColor.c_Red, TMsgType.t_Hint);
            //    return;
            //}
            //if (!PlayObject.m_PEnvir.m_boFight3Zone)
            //{
            //    PlayObject.SysMsg("此命令不能在当前地图中使用!!!", TMsgColor.c_Red, TMsgType.t_Hint);
            //    return;
            //}
            //Guild = M2Share.g_GuildManager.FindGuild(sGuildName);
            //if (Guild != null)
            //{
            //    M2Share.UserEngine.CryCry(Grobal2.RM_CRY, PlayObject.m_PEnvir, PlayObject.m_nCurrX, PlayObject.m_nCurrY, 1000, M2Share.g_Config.btCryMsgFColor,
            //        M2Share.g_Config.btCryMsgBColor, string.Format(" - %s 行会争霸赛结果: ", Guild.sGuildName));
            //    for (int i = 0; i < Guild.TeamFightDeadList.Count; i++)
            //    {
            //        nPoint = HUtil32.ObjectToInt(Guild.TeamFightDeadList[i]);
            //        sHumanName = Guild.TeamFightDeadList[i];
            //        M2Share.UserEngine.CryCry(Grobal2.RM_CRY, PlayObject.m_PEnvir, PlayObject.m_nCurrX, PlayObject.m_nCurrY, 1000,
            //            M2Share.g_Config.btCryMsgFColor, M2Share.g_Config.btCryMsgBColor, string.Format(" - %s  : %d 分/死亡%d次。 ",
            //            sHumanName, HUtil32.HiWord(nPoint), HUtil32.LoWord(nPoint)));
            //    }
            //}
            //M2Share.UserEngine.CryCry(Grobal2.RM_CRY, PlayObject.m_PEnvir, PlayObject.m_nCurrX, PlayObject.m_nCurrY, 1000,
            //    M2Share.g_Config.btCryMsgFColor, M2Share.g_Config.btCryMsgBColor, string.Format(" - [%s] : %d 分。", Guild.sGuildName, Guild.nContestPoint));
            //M2Share.UserEngine.CryCry(Grobal2.RM_CRY, PlayObject.m_PEnvir, PlayObject.m_nCurrX, PlayObject.m_nCurrY, 1000,
            //    M2Share.g_Config.btCryMsgFColor, M2Share.g_Config.btCryMsgBColor, "------------------------------------");
        }
    }
}