﻿using System.Globalization;
using SystemModule;

namespace GameSvr
{
    public partial class TPlayObject
    {
        protected virtual void Whisper(string whostr, string saystr)
        {
            var svidx = 0;
            var PlayObject = M2Share.UserEngine.GetPlayObject(whostr);
            if (PlayObject != null)
            {
                if (!PlayObject.m_boReadyRun)
                {
                    SysMsg(whostr + M2Share.g_sCanotSendmsg, MsgColor.Red, MsgType.Hint);
                    return;
                }
                if (!PlayObject.m_boHearWhisper || PlayObject.IsBlockWhisper(m_sCharName))
                {
                    SysMsg(whostr + M2Share.g_sUserDenyWhisperMsg, MsgColor.Red, MsgType.Hint);
                    return;
                }
                if (!m_boOffLineFlag && PlayObject.m_boOffLineFlag)
                {
                    if (PlayObject.m_sOffLineLeaveword != "")
                    {
                        PlayObject.Whisper(m_sCharName, PlayObject.m_sOffLineLeaveword);
                    }
                    else
                    {
                        PlayObject.Whisper(m_sCharName, M2Share.g_Config.sServerName + '[' + M2Share.g_Config.sServerIPaddr + "]提示您");
                    }
                    return;
                }
                if (m_btPermission > 0)
                {
                    PlayObject.SendMsg(PlayObject, Grobal2.RM_WHISPER, 0, M2Share.g_Config.btGMWhisperMsgFColor, M2Share.g_Config.btGMWhisperMsgBColor, 0, m_sCharName + "=> " + saystr);
                    if (m_GetWhisperHuman != null && !m_GetWhisperHuman.m_boGhost)
                    {
                        m_GetWhisperHuman.SendMsg(m_GetWhisperHuman, Grobal2.RM_WHISPER, 0, M2Share.g_Config.btGMWhisperMsgFColor, M2Share.g_Config.btGMWhisperMsgBColor, 0, m_sCharName + "=>" + PlayObject.m_sCharName + ' ' + saystr);
                    }
                    if (PlayObject.m_GetWhisperHuman != null && !PlayObject.m_GetWhisperHuman.m_boGhost)
                    {
                        PlayObject.m_GetWhisperHuman.SendMsg(PlayObject.m_GetWhisperHuman, Grobal2.RM_WHISPER, 0, M2Share.g_Config.btGMWhisperMsgFColor, M2Share.g_Config.btGMWhisperMsgBColor, 0, m_sCharName + "=>" + PlayObject.m_sCharName + ' ' + saystr);
                    }
                }
                else
                {
                    PlayObject.SendMsg(PlayObject, Grobal2.RM_WHISPER, 0, M2Share.g_Config.btWhisperMsgFColor, M2Share.g_Config.btWhisperMsgBColor, 0, m_sCharName + "=> " + saystr);
                    if (m_GetWhisperHuman != null && !m_GetWhisperHuman.m_boGhost)
                    {
                        m_GetWhisperHuman.SendMsg(m_GetWhisperHuman, Grobal2.RM_WHISPER, 0, M2Share.g_Config.btWhisperMsgFColor, M2Share.g_Config.btWhisperMsgBColor, 0, m_sCharName + "=>" + PlayObject.m_sCharName + ' ' + saystr);
                    }
                    if (PlayObject.m_GetWhisperHuman != null && !PlayObject.m_GetWhisperHuman.m_boGhost)
                    {
                        PlayObject.m_GetWhisperHuman.SendMsg(PlayObject.m_GetWhisperHuman, Grobal2.RM_WHISPER, 0, M2Share.g_Config.btWhisperMsgFColor, M2Share.g_Config.btWhisperMsgBColor, 0, m_sCharName + "=>" + PlayObject.m_sCharName + ' ' + saystr);
                    }
                }
            }
            else
            {
                if (M2Share.UserEngine.FindOtherServerUser(whostr, ref svidx))
                {
                    M2Share.UserEngine.SendServerGroupMsg(Grobal2.ISM_WHISPER, svidx, whostr + '/' + m_sCharName + "=> " + saystr);
                }
                else
                {
                    SysMsg(whostr + M2Share.g_sUserNotOnLine, MsgColor.Red, MsgType.Hint);
                }
            }
        }

        public void WhisperRe(string SayStr, byte MsgType)
        {
            var sendwho = string.Empty;
            HUtil32.GetValidStr3(SayStr, ref sendwho, new string[] { "[", " ", "=", ">" });
            if (m_boHearWhisper && !IsBlockWhisper(sendwho))
            {
                switch (MsgType)
                {
                    case 0:
                        SendMsg(this, Grobal2.RM_WHISPER, 0, M2Share.g_Config.btGMWhisperMsgFColor,
                            M2Share.g_Config.btGMWhisperMsgBColor, 0, SayStr);
                        break;
                    case 1:
                        SendMsg(this, Grobal2.RM_WHISPER, 0, M2Share.g_Config.btWhisperMsgFColor,
                            M2Share.g_Config.btWhisperMsgBColor, 0, SayStr);
                        break;
                    case 2:
                        SendMsg(this, Grobal2.RM_WHISPER, 0, M2Share.g_Config.btPurpleMsgFColor,
                            M2Share.g_Config.btPurpleMsgBColor, 0, SayStr);
                        break;
                }
            }
        }

        /// <summary>
        /// 处理玩家说话
        /// </summary>
        /// <param name="sData"></param>
        protected override void ProcessSayMsg(string sData)
        {
            bool boDisableSayMsg;
            var sC = string.Empty;
            var sCryCryMsg = string.Empty;
            var sParam1 = string.Empty;
            const string sExceptionMsg = "[Exception] TPlayObject.ProcessSayMsg Msg = {0}";
            try
            {
                if (sData.Length > M2Share.g_Config.nSayMsgMaxLen)
                {
                    sData = sData.Substring(0, M2Share.g_Config.nSayMsgMaxLen); // 3 * 1000
                }
                if ((HUtil32.GetTickCount() - m_dwSayMsgTick) < M2Share.g_Config.dwSayMsgTime)
                {
                    m_nSayMsgCount++;// 2
                    if (m_nSayMsgCount >= M2Share.g_Config.nSayMsgCount)
                    {
                        m_boDisableSayMsg = true;
                        m_dwDisableSayMsgTick = HUtil32.GetTickCount() + M2Share.g_Config.dwDisableSayMsgTime;// 60 * 1000
                        SysMsg(format(M2Share.g_sDisableSayMsg, M2Share.g_Config.dwDisableSayMsgTime / (60 * 1000)), MsgColor.Red, MsgType.Hint);
                    }
                }
                else
                {
                    m_dwSayMsgTick = HUtil32.GetTickCount();
                    m_nSayMsgCount = 0;
                }
                if (HUtil32.GetTickCount() >= m_dwDisableSayMsgTick)
                {
                    m_boDisableSayMsg = false;
                }
                boDisableSayMsg = m_boDisableSayMsg;
                if (M2Share.g_DenySayMsgList.ContainsKey(this.m_sCharName))
                {
                    boDisableSayMsg = true;
                }
                if (!(boDisableSayMsg || m_PEnvir.Flag.boNOCHAT))
                {
                    M2Share.g_ChatLoggingList.Add('[' + DateTime.Now.ToString(CultureInfo.InvariantCulture) + "] " + m_sCharName + ": " + sData);
                    m_sOldSayMsg = sData;
                    if (sData.StartsWith("@@加速处理"))
                    {
                        M2Share.g_FunctionNPC.GotoLable(this, "@加速处理", false);
                        return;
                    }
                    switch (sData[0])
                    {
                        case '/':
                            {
                                sC = sData.Substring(1, sData.Length - 1);
                                //if (string.Compare(sC.Trim(), M2Share.g_GameCommand.WHO.sCmd.Trim(), StringComparison.OrdinalIgnoreCase) == 0)
                                //{
                                //    if (m_btPermission < M2Share.g_GameCommand.WHO.nPerMissionMin)
                                //    {
                                //        SysMsg(M2Share.g_sGameCommandPermissionTooLow, TMsgColor.c_Red, TMsgType.t_Hint);
                                //        return;
                                //    }
                                //    HearMsg(format(M2Share.g_sOnlineCountMsg, M2Share.UserEngine.PlayObjectCount));
                                //    return;
                                //}
                                //if (string.Compare(sC.Trim(), M2Share.g_GameCommand.TOTAL.sCmd.Trim(), StringComparison.OrdinalIgnoreCase) == 0) //统计在线人数
                                //{
                                //    if (m_btPermission < M2Share.g_GameCommand.TOTAL.nPerMissionMin)
                                //    {
                                //        SysMsg(M2Share.g_sGameCommandPermissionTooLow, TMsgColor.c_Red, TMsgType.t_Hint);
                                //        return;
                                //    }
                                //    HearMsg(format(M2Share.g_sTotalOnlineCountMsg, M2Share.g_nTotalHumCount));
                                //    return;
                                //}
                                sC = HUtil32.GetValidStr3(sC, ref sParam1, new[] { " " });
                                if (!m_boFilterSendMsg)
                                {
                                    Whisper(sParam1, sC);
                                }
                                return;
                            }
                        case '!':
                            {
                                if (sData.Length >= 1)
                                {
                                    if (sData[1] == '!') //发送组队消息
                                    {
                                        sC = sData.Substring(3 - 1, sData.Length - 2);
                                        SendGroupText(m_sCharName + ": " + sC);
                                        M2Share.UserEngine.SendServerGroupMsg(Grobal2.SS_208, M2Share.nServerIndex, m_sCharName + "/:" + sC);
                                        return;
                                    }
                                    if (sData[1] == '~' && m_MyGuild != null) //发送行会消息
                                    {
                                        sC = sData.Substring(2, sData.Length - 2);
                                        m_MyGuild.SendGuildMsg(m_sCharName + ": " + sC);
                                        M2Share.UserEngine.SendServerGroupMsg(Grobal2.SS_208, M2Share.nServerIndex, m_MyGuild.sGuildName + '/' + m_sCharName + '/' + sC);
                                        return;
                                    }
                                }
                                if (!m_PEnvir.Flag.boQUIZ)
                                {
                                    if ((HUtil32.GetTickCount() - m_dwShoutMsgTick) > 10 * 1000)
                                    {
                                        if (m_Abil.Level <= M2Share.g_Config.nCanShoutMsgLevel)
                                        {
                                            SysMsg(format(M2Share.g_sYouNeedLevelMsg, M2Share.g_Config.nCanShoutMsgLevel + 1), MsgColor.Red, MsgType.Hint);
                                            return;
                                        }
                                        m_dwShoutMsgTick = HUtil32.GetTickCount();
                                        sC = sData.Substring(1, sData.Length - 1);
                                        sCryCryMsg = "(!)" + m_sCharName + ": " + sC;
                                        if (m_boFilterSendMsg)
                                        {
                                            SendMsg(null, Grobal2.RM_CRY, 0, 0, 0xFFFF, 0, sCryCryMsg);
                                        }
                                        else
                                        {
                                            M2Share.UserEngine.CryCry(Grobal2.RM_CRY, m_PEnvir, m_nCurrX, m_nCurrY, 50, M2Share.g_Config.btCryMsgFColor, M2Share.g_Config.btCryMsgBColor, sCryCryMsg);
                                        }
                                        return;
                                    }
                                    SysMsg(format(M2Share.g_sYouCanSendCyCyLaterMsg, new[] { 10 - (HUtil32.GetTickCount() - m_dwShoutMsgTick) / 1000 }), MsgColor.Red, MsgType.Hint);
                                    return;
                                }
                                SysMsg(M2Share.g_sThisMapDisableSendCyCyMsg, MsgColor.Red, MsgType.Hint);
                                return;
                            }
                    }
                    if (m_boFilterSendMsg)
                    {
                        SendMsg(this, Grobal2.RM_HEAR, 0, M2Share.g_Config.btHearMsgFColor, M2Share.g_Config.btHearMsgBColor, 0, m_sCharName + ':' + sData);// 如果禁止发信息，则只向自己发信息
                    }
                    else
                    {
                        base.ProcessSayMsg(sData);
                    }
                    return;
                }
                SysMsg(M2Share.g_sYouIsDisableSendMsg, MsgColor.Red, MsgType.Hint);
            }
            catch (Exception e)
            {
                M2Share.ErrorMessage(format(sExceptionMsg, sData));
                M2Share.ErrorMessage(e.StackTrace);
            }
        }

        internal void ProcessUserLineMsg(string sData)
        {
            string sC;
            var sCMD = string.Empty;
            var sParam1 = string.Empty;
            var sParam2 = string.Empty;
            var sParam3 = string.Empty;
            var sParam4 = string.Empty;
            var sParam5 = string.Empty;
            var sParam6 = string.Empty;
            var sParam7 = string.Empty;
            TPlayObject PlayObject;
            int nFlag;
            int nValue;
            int nLen;
            const string sExceptionMsg = "[Exception] TPlayObject::ProcessUserLineMsg Msg = {0}";
            try
            {
                nLen = sData.Length;
                if (nLen <= 0)
                {
                    return;
                }
                if (m_boSetStoragePwd)
                {
                    m_boSetStoragePwd = false;
                    if (nLen > 3 && nLen < 8)
                    {
                        m_sTempPwd = sData;
                        m_boReConfigPwd = true;
                        SysMsg(M2Share.g_sReSetPasswordMsg, MsgColor.Green, MsgType.Hint);
                        SendMsg(this, Grobal2.RM_PASSWORD, 0, 0, 0, 0, "");
                    }
                    else
                    {
                        SysMsg(M2Share.g_sPasswordOverLongMsg, MsgColor.Red, MsgType.Hint);// '输入的密码长度不正确!!!，密码长度必须在 4 - 7 的范围内，请重新设置密码。'
                    }
                    return;
                }
                if (m_boReConfigPwd)
                {
                    m_boReConfigPwd = false;
                    if (string.Compare(m_sTempPwd, sData, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        m_sStoragePwd = sData;
                        m_boPasswordLocked = true;
                        m_boCanGetBackItem = false;
                        m_sTempPwd = "";
                        SysMsg(M2Share.g_sReSetPasswordOKMsg, MsgColor.Blue, MsgType.Hint);
                    }
                    else
                    {
                        m_sTempPwd = "";
                        SysMsg(M2Share.g_sReSetPasswordNotMatchMsg, MsgColor.Red, MsgType.Hint);
                    }
                    return;
                }
                if (m_boUnLockPwd || m_boUnLockStoragePwd)
                {
                    if (string.Compare(m_sStoragePwd, sData, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        m_boPasswordLocked = false;
                        if (m_boUnLockPwd)
                        {
                            if (M2Share.g_Config.boLockDealAction)
                            {
                                m_boCanDeal = true;
                            }
                            if (M2Share.g_Config.boLockDropAction)
                            {
                                m_boCanDrop = true;
                            }
                            if (M2Share.g_Config.boLockWalkAction)
                            {
                                m_boCanWalk = true;
                            }
                            if (M2Share.g_Config.boLockRunAction)
                            {
                                m_boCanRun = true;
                            }
                            if (M2Share.g_Config.boLockHitAction)
                            {
                                m_boCanHit = true;
                            }
                            if (M2Share.g_Config.boLockSpellAction)
                            {
                                m_boCanSpell = true;
                            }
                            if (M2Share.g_Config.boLockSendMsgAction)
                            {
                                m_boCanSendMsg = true;
                            }
                            if (M2Share.g_Config.boLockUserItemAction)
                            {
                                m_boCanUseItem = true;
                            }
                            if (M2Share.g_Config.boLockInObModeAction)
                            {
                                m_boObMode = false;
                                m_boAdminMode = false;
                            }
                            m_boLockLogoned = true;
                            SysMsg(M2Share.g_sPasswordUnLockOKMsg, MsgColor.Blue, MsgType.Hint);
                        }
                        if (m_boUnLockStoragePwd)
                        {
                            if (M2Share.g_Config.boLockGetBackItemAction)
                            {
                                m_boCanGetBackItem = true;
                            }
                            SysMsg(M2Share.g_sStorageUnLockOKMsg, MsgColor.Blue, MsgType.Hint);
                        }
                    }
                    else
                    {
                        m_btPwdFailCount++;
                        SysMsg(M2Share.g_sUnLockPasswordFailMsg, MsgColor.Red, MsgType.Hint);
                        if (m_btPwdFailCount > 3)
                        {
                            SysMsg(M2Share.g_sStoragePasswordLockedMsg, MsgColor.Red, MsgType.Hint);
                        }
                    }
                    m_boUnLockPwd = false;
                    m_boUnLockStoragePwd = false;
                    return;
                }
                if (m_boCheckOldPwd)
                {
                    m_boCheckOldPwd = false;
                    if (m_sStoragePwd == sData)
                    {
                        SendMsg(this, Grobal2.RM_PASSWORD, 0, 0, 0, 0, "");
                        SysMsg(M2Share.g_sSetPasswordMsg, MsgColor.Green, MsgType.Hint);
                        m_boSetStoragePwd = true;
                    }
                    else
                    {
                        m_btPwdFailCount++;
                        SysMsg(M2Share.g_sOldPasswordIncorrectMsg, MsgColor.Red, MsgType.Hint);
                        if (m_btPwdFailCount > 3)
                        {
                            SysMsg(M2Share.g_sStoragePasswordLockedMsg, MsgColor.Red, MsgType.Hint);
                            m_boPasswordLocked = true;
                        }
                    }
                    return;
                }
                if (!sData.StartsWith("@"))
                {
                    ProcessSayMsg(sData);
                    return;
                }
                sC = sData.Substring(1, sData.Length - 1);
                sC = HUtil32.GetValidStr3(sC, ref sCMD, new[] { " ", ":", ",", "\t" });
                if (sC != "")
                {
                    sC = HUtil32.GetValidStr3(sC, ref sParam1, new[] { " ", ":", ",", "\t" });
                }
                if (sC != "")
                {
                    sC = HUtil32.GetValidStr3(sC, ref sParam2, new[] { " ", ":", ",", "\t" });
                }
                if (sC != "")
                {
                    sC = HUtil32.GetValidStr3(sC, ref sParam3, new[] { " ", ":", ",", "\t" });
                }
                if (sC != "")
                {
                    sC = HUtil32.GetValidStr3(sC, ref sParam4, new[] { " ", ":", ",", "\t" });
                }
                if (sC != "")
                {
                    sC = HUtil32.GetValidStr3(sC, ref sParam5, new[] { " ", ":", ",", "\t" });
                }
                if (sC != "")
                {
                    sC = HUtil32.GetValidStr3(sC, ref sParam6, new[] { " ", ":", ",", "\t" });
                }
                if (sC != "")
                {
                    sC = HUtil32.GetValidStr3(sC, ref sParam7, new[] { " ", ":", ",", "\t" });
                }
                // 新密码命令
                if (string.Compare(sCMD, M2Share.g_GameCommand.PASSWORDLOCK.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (!M2Share.g_Config.boPasswordLockSystem)
                    {
                        SysMsg(M2Share.g_sNoPasswordLockSystemMsg, MsgColor.Red, MsgType.Hint);
                        return;
                    }
                    if (m_sStoragePwd == "")
                    {
                        SendMsg(this, Grobal2.RM_PASSWORD, 0, 0, 0, 0, "");
                        m_boSetStoragePwd = true;
                        SysMsg(M2Share.g_sSetPasswordMsg, MsgColor.Green, MsgType.Hint);
                        return;
                    }
                    if (m_btPwdFailCount > 3)
                    {
                        SysMsg(M2Share.g_sStoragePasswordLockedMsg, MsgColor.Red, MsgType.Hint);
                        m_boPasswordLocked = true;
                        return;
                    }
                    if (!string.IsNullOrEmpty(m_sStoragePwd))
                    {
                        SendMsg(this, Grobal2.RM_PASSWORD, 0, 0, 0, 0, "");
                        m_boCheckOldPwd = true;
                        SysMsg(M2Share.g_sPleaseInputOldPasswordMsg, MsgColor.Green, MsgType.Hint);
                        return;
                    }
                    return;
                }
                if (M2Share.CommandSystem.ExecCmd(sData, this))
                {
                    return;
                }
                // 新密码命令
                if (string.Compare(sCMD, M2Share.g_GameCommand.SETPASSWORD.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (!M2Share.g_Config.boPasswordLockSystem)
                    {
                        SysMsg(M2Share.g_sNoPasswordLockSystemMsg, MsgColor.Red, MsgType.Hint);
                        return;
                    }
                    if (m_sStoragePwd == "")
                    {
                        SendMsg(this, Grobal2.RM_PASSWORD, 0, 0, 0, 0, "");
                        m_boSetStoragePwd = true;
                        SysMsg(M2Share.g_sSetPasswordMsg, MsgColor.Green, MsgType.Hint);
                    }
                    else
                    {
                        SysMsg(M2Share.g_sAlreadySetPasswordMsg, MsgColor.Red, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.UNPASSWORD.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (!M2Share.g_Config.boPasswordLockSystem)
                    {
                        SysMsg(M2Share.g_sNoPasswordLockSystemMsg, MsgColor.Red, MsgType.Hint);
                        return;
                    }
                    if (!m_boPasswordLocked)
                    {
                        m_sStoragePwd = "";
                        SysMsg(M2Share.g_sOldPasswordIsClearMsg, MsgColor.Green, MsgType.Hint);
                    }
                    else
                    {
                        SysMsg(M2Share.g_sPleaseUnLockPasswordMsg, MsgColor.Red, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.CHGPASSWORD.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (!M2Share.g_Config.boPasswordLockSystem)
                    {
                        SysMsg(M2Share.g_sNoPasswordLockSystemMsg, MsgColor.Red, MsgType.Hint);
                        return;
                    }
                    if (m_btPwdFailCount > 3)
                    {
                        SysMsg(M2Share.g_sStoragePasswordLockedMsg, MsgColor.Red, MsgType.Hint);
                        m_boPasswordLocked = true;
                        return;
                    }
                    if (m_sStoragePwd != "")
                    {
                        SendMsg(this, Grobal2.RM_PASSWORD, 0, 0, 0, 0, "");
                        m_boCheckOldPwd = true;
                        SysMsg(M2Share.g_sPleaseInputOldPasswordMsg, MsgColor.Green, MsgType.Hint);
                    }
                    else
                    {
                        SysMsg(M2Share.g_sNoPasswordSetMsg, MsgColor.Red, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.UNLOCKSTORAGE.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (!M2Share.g_Config.boPasswordLockSystem)
                    {
                        SysMsg(M2Share.g_sNoPasswordLockSystemMsg, MsgColor.Red, MsgType.Hint);
                        return;
                    }
                    if (m_btPwdFailCount > M2Share.g_Config.nPasswordErrorCountLock)
                    {
                        SysMsg(M2Share.g_sStoragePasswordLockedMsg, MsgColor.Red, MsgType.Hint);
                        m_boPasswordLocked = true;
                        return;
                    }
                    if (m_sStoragePwd != "")
                    {
                        if (!m_boUnLockStoragePwd)
                        {
                            SendMsg(this, Grobal2.RM_PASSWORD, 0, 0, 0, 0, "");
                            SysMsg(M2Share.g_sPleaseInputUnLockPasswordMsg, MsgColor.Green, MsgType.Hint);
                            m_boUnLockStoragePwd = true;
                        }
                        else
                        {
                            SysMsg(M2Share.g_sStorageAlreadyUnLockMsg, MsgColor.Red, MsgType.Hint);
                        }
                    }
                    else
                    {
                        SysMsg(M2Share.g_sStorageNoPasswordMsg, MsgColor.Red, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.UNLOCK.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (!M2Share.g_Config.boPasswordLockSystem)
                    {
                        SysMsg(M2Share.g_sNoPasswordLockSystemMsg, MsgColor.Red, MsgType.Hint);
                        return;
                    }
                    if (m_btPwdFailCount > M2Share.g_Config.nPasswordErrorCountLock)
                    {
                        SysMsg(M2Share.g_sStoragePasswordLockedMsg, MsgColor.Red, MsgType.Hint);
                        m_boPasswordLocked = true;
                        return;
                    }
                    if (m_sStoragePwd != "")
                    {
                        if (!m_boUnLockPwd)
                        {
                            SendMsg(this, Grobal2.RM_PASSWORD, 0, 0, 0, 0, "");
                            SysMsg(M2Share.g_sPleaseInputUnLockPasswordMsg, MsgColor.Green, MsgType.Hint);
                            m_boUnLockPwd = true;
                        }
                        else
                        {
                            SysMsg(M2Share.g_sStorageAlreadyUnLockMsg, MsgColor.Red, MsgType.Hint);
                        }
                    }
                    else
                    {
                        SysMsg(M2Share.g_sStorageNoPasswordMsg, MsgColor.Red, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.__LOCK.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (!M2Share.g_Config.boPasswordLockSystem)
                    {
                        SysMsg(M2Share.g_sNoPasswordLockSystemMsg, MsgColor.Red, MsgType.Hint);
                        return;
                    }
                    if (!m_boPasswordLocked)
                    {
                        if (m_sStoragePwd != "")
                        {
                            m_boPasswordLocked = true;
                            m_boCanGetBackItem = false;
                            SysMsg(M2Share.g_sLockStorageSuccessMsg, MsgColor.Green, MsgType.Hint);
                        }
                        else
                        {
                            SysMsg(M2Share.g_sStorageNoPasswordMsg, MsgColor.Green, MsgType.Hint);
                        }
                    }
                    else
                    {
                        SysMsg(M2Share.g_sStorageAlreadyLockMsg, MsgColor.Red, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.ALLOWDEARRCALL.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    m_boCanDearRecall = !m_boCanDearRecall;
                    if (m_boCanDearRecall)
                    {
                        SysMsg(M2Share.g_sEnableDearRecall, MsgColor.Blue, MsgType.Hint);
                    }
                    else
                    {
                        SysMsg(M2Share.g_sDisableDearRecall, MsgColor.Blue, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.ALLOWMASTERRECALL.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    m_boCanMasterRecall = !m_boCanMasterRecall;
                    if (m_boCanMasterRecall)
                    {
                        SysMsg(M2Share.g_sEnableMasterRecall, MsgColor.Blue, MsgType.Hint);
                    }
                    else
                    {
                        SysMsg(M2Share.g_sDisableMasterRecall, MsgColor.Blue, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.DATA.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    SysMsg(M2Share.g_sNowCurrDateTime + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), MsgColor.Blue, MsgType.Hint);
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.ALLOWMSG.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    m_boHearWhisper = !m_boHearWhisper;
                    if (m_boHearWhisper)
                    {
                        SysMsg(M2Share.g_sEnableHearWhisper, MsgColor.Green, MsgType.Hint);
                    }
                    else
                    {
                        SysMsg(M2Share.g_sDisableHearWhisper, MsgColor.Green, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.LETSHOUT.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    m_boBanShout = !m_boBanShout;
                    if (m_boBanShout)
                    {
                        SysMsg(M2Share.g_sEnableShoutMsg, MsgColor.Green, MsgType.Hint);
                    }
                    else
                    {
                        SysMsg(M2Share.g_sDisableShoutMsg, MsgColor.Green, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.LETTRADE.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    m_boAllowDeal = !m_boAllowDeal;
                    if (m_boAllowDeal)
                    {
                        SysMsg(M2Share.g_sEnableDealMsg, MsgColor.Green, MsgType.Hint);
                    }
                    else
                    {
                        SysMsg(M2Share.g_sDisableDealMsg, MsgColor.Green, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.BANGUILDCHAT.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    m_boBanGuildChat = !m_boBanGuildChat;
                    if (m_boBanGuildChat)
                    {
                        SysMsg(M2Share.g_sEnableGuildChat, MsgColor.Green, MsgType.Hint);
                    }
                    else
                    {
                        SysMsg(M2Share.g_sDisableGuildChat, MsgColor.Green, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.LETGUILD.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    m_boAllowGuild = !m_boAllowGuild;
                    if (m_boAllowGuild)
                    {
                        SysMsg(M2Share.g_sEnableJoinGuild, MsgColor.Green, MsgType.Hint);
                    }
                    else
                    {
                        SysMsg(M2Share.g_sDisableJoinGuild, MsgColor.Green, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.AUTHALLY.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (IsGuildMaster())
                    {
                        m_MyGuild.m_boEnableAuthAlly = !m_MyGuild.m_boEnableAuthAlly;
                        if (m_MyGuild.m_boEnableAuthAlly)
                        {
                            SysMsg(M2Share.g_sEnableAuthAllyGuild, MsgColor.Green, MsgType.Hint);
                        }
                        else
                        {
                            SysMsg(M2Share.g_sDisableAuthAllyGuild, MsgColor.Green, MsgType.Hint);
                        }
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.ALLOWGUILDRECALL.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    m_boAllowGuildReCall = !m_boAllowGuildReCall;
                    if (m_boAllowGuildReCall)
                    {
                        SysMsg(M2Share.g_sEnableGuildRecall, MsgColor.Green, MsgType.Hint);
                    }
                    else
                    {
                        SysMsg(M2Share.g_sDisableGuildRecall, MsgColor.Green, MsgType.Hint);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.AUTH.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (IsGuildMaster())
                    {
                        ClientGuildAlly();
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.AUTHCANCEL.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (IsGuildMaster())
                    {
                        ClientGuildBreakAlly(sParam1);
                    }
                    return;
                }
                if (string.Compare(sCMD, M2Share.g_GameCommand.MAPINFO.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ShowMapInfo(sParam1, sParam2, sParam3);
                    return;
                }
                if (m_btPermission >= 2 && sData.Length > 2)
                {
                    if (m_btPermission >= 6 && sData[2] == M2Share.g_GMRedMsgCmd)
                    {
                        if (HUtil32.GetTickCount() - m_dwSayMsgTick > 2000)
                        {
                            m_dwSayMsgTick = HUtil32.GetTickCount();
                            sData = sData.Substring(2, sData.Length - 2);
                            if (sData.Length > M2Share.g_Config.nSayRedMsgMaxLen)
                            {
                                sData = sData.Substring(0, M2Share.g_Config.nSayRedMsgMaxLen);
                            }
                            if (M2Share.g_Config.boShutRedMsgShowGMName)
                            {
                                sC = m_sCharName + ": " + sData;
                            }
                            else
                            {
                                sC = sData;
                            }
                            M2Share.UserEngine.SendBroadCastMsg(sC, MsgType.GM);
                        }
                        return;
                    }
                }
                if (m_btPermission > 4)
                {
                    if (string.Compare(sCMD, M2Share.g_GameCommand.SETFLAG.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        PlayObject = M2Share.UserEngine.GetPlayObject(sParam1);
                        if (PlayObject != null)
                        {
                            nFlag = HUtil32.Str_ToInt(sParam2, 0);
                            nValue = HUtil32.Str_ToInt(sParam3, 0);
                            PlayObject.SetQuestFlagStatus(nFlag, nValue);
                            if (PlayObject.GetQuestFalgStatus(nFlag) == 1)
                            {
                                SysMsg(PlayObject.m_sCharName + ": [" + nFlag + "] = ON", MsgColor.Green, MsgType.Hint);
                            }
                            else
                            {
                                SysMsg(PlayObject.m_sCharName + ": [" + nFlag + "] = OFF", MsgColor.Green, MsgType.Hint);
                            }
                        }
                        else
                        {
                            SysMsg('@' + M2Share.g_GameCommand.SETFLAG.sCmd + " 人物名称 标志号 数字(0 - 1)", MsgColor.Red, MsgType.Hint);
                        }
                        return;
                    }
                    if (string.Compare(sCMD, M2Share.g_GameCommand.SETOPEN.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        PlayObject = M2Share.UserEngine.GetPlayObject(sParam1);
                        if (PlayObject != null)
                        {
                            nFlag = HUtil32.Str_ToInt(sParam2, 0);
                            nValue = HUtil32.Str_ToInt(sParam3, 0);
                            PlayObject.SetQuestUnitOpenStatus(nFlag, nValue);
                            if (PlayObject.GetQuestUnitOpenStatus(nFlag) == 1)
                            {
                                SysMsg(PlayObject.m_sCharName + ": [" + nFlag + "] = ON", MsgColor.Green, MsgType.Hint);
                            }
                            else
                            {
                                SysMsg(PlayObject.m_sCharName + ": [" + nFlag + "] = OFF", MsgColor.Green, MsgType.Hint);
                            }
                        }
                        else
                        {
                            SysMsg('@' + M2Share.g_GameCommand.SETOPEN.sCmd + " 人物名称 标志号 数字(0 - 1)", MsgColor.Red, MsgType.Hint);
                        }
                        return;
                    }
                    if (string.Compare(sCMD, M2Share.g_GameCommand.SETUNIT.sCmd, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        PlayObject = M2Share.UserEngine.GetPlayObject(sParam1);
                        if (PlayObject != null)
                        {
                            nFlag = HUtil32.Str_ToInt(sParam2, 0);
                            nValue = HUtil32.Str_ToInt(sParam3, 0);
                            PlayObject.SetQuestUnitStatus(nFlag, nValue);
                            if (PlayObject.GetQuestUnitStatus(nFlag) == 1)
                            {
                                SysMsg(PlayObject.m_sCharName + ": [" + nFlag + "] = ON", MsgColor.Green, MsgType.Hint);
                            }
                            else
                            {
                                SysMsg(PlayObject.m_sCharName + ": [" + nFlag + "] = OFF", MsgColor.Green, MsgType.Hint);
                            }
                        }
                        else
                        {
                            SysMsg('@' + M2Share.g_GameCommand.SETUNIT.sCmd + " 人物名称 标志号 数字(0 - 1)", MsgColor.Red, MsgType.Hint);
                        }
                        return;
                    }
                }
                SysMsg($"@{sCMD}此命令不正确，或没有足够的权限!!!", MsgColor.Red, MsgType.Hint);
            }
            catch (Exception e)
            {
                M2Share.ErrorMessage(format(sExceptionMsg, sData));
                M2Share.ErrorMessage(e.Message);
            }
        }
    }
}