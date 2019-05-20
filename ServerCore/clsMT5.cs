using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;


namespace ServerCore
{
    public class clsMT5 : IDisposable
    {
        //--- Manager API
        public CIMTManagerAPI m_manager = null;

        public MTRetCode Initialize()
        {

            MTRetCode res = MTRetCode.MT_RET_ERROR;

            //--- Initialize the factory
            if ((res = SMTManagerAPIFactory.Initialize(@"..\..\API\")) != MTRetCode.MT_RET_OK)
            {
                LogOut($"SMTManagerAPIFactory.Initialize failed - {res}");
                return (res);
            }

            //--- Receive the API version
            uint version = 0;
            if ((res = SMTManagerAPIFactory.GetVersion(out version)) != MTRetCode.MT_RET_OK)
            {
                LogOut($"SMTManagerAPIFactory.GetVersion failed - {res}");
                return (res);
            }
            //--- Compare the obtained version with the library one
            if (version != SMTManagerAPIFactory.ManagerAPIVersion)
            {
                LogOut($"Manager API version mismatch - {version}!={SMTManagerAPIFactory.ManagerAPIVersion}");
                return (MTRetCode.MT_RET_ERROR);
            }
            //--- Create an instance
            m_manager = SMTManagerAPIFactory.CreateManager(SMTManagerAPIFactory.ManagerAPIVersion, out res);
            if (res != MTRetCode.MT_RET_OK)
            {
                LogOut($"SMTManagerAPIFactory.CreateManager failed - {res}");
                return (res);
            }

            //--- For some reasons, the creation method returned OK and the null pointer
            if (m_manager == null)
            {
                LogOut("SMTManagerAPIFactory.CreateManager was ok, but ManagerAPI is null");
                return (MTRetCode.MT_RET_ERR_MEM);
            }

            //--- All is well
            LogOut($"Using ManagerAPI v. {version}");
            return (res);
        }


        //Do all stuff to shut down connection with server
        public void Shutdown() {
            try
            {
                LogOut("Shuting down");
                this.m_manager.Disconnect();
                this.m_manager.Release();
                SMTManagerAPIFactory.Shutdown();
            }
            catch 
            {
                //throw;
            }
        }

        //publish out information
        private void LogOut(string myText)
        {
            Console.WriteLine(myText);
        }

        //return True if Mannager is ready to workd
        private bool CheckIfMannagerWorks()
        {
            if (m_manager == null)
            {
                LogOut("ERROR: Manager was not created");
                return false;
            }

            return true;
        }

        //return list of all groups at given server
        public List<CIMTConGroup> GetAllGroups()
        {
            MTRetCode res = MTRetCode.MT_RET_ERROR;
            return this.GetAllGroups(out res);
        }


        //return list of all groups at given server and success state too
        public List<CIMTConGroup> GetAllGroups(out MTRetCode requestResult)
        {
            List<CIMTConGroup> result = new List<CIMTConGroup>();
            requestResult = MTRetCode.MT_RET_ERROR;
            if (!CheckIfMannagerWorks()) return result;

            try
            {
                var serverGroupCount = m_manager.GroupTotal();
                for (uint i = 0; i < serverGroupCount; i++)
                {
                    var actGroup = m_manager.GroupCreate();
                    requestResult = m_manager.GroupNext(i, actGroup);

                    if (requestResult == MTRetCode.MT_RET_OK)
                    {
                        result.Add(actGroup);
                    }
                    else
                    {
                        LogOut($"Error getting group: group_id={i} error msg={requestResult}");
                    }
                }
            }
            catch (Exception ex)
            {

                LogOut($"Error getting group:  error={ex}");
            }

            return result;
        }

        //return list of all users at given group
        public List<CIMTUser> GetAllUsersAtGroup(string GroupName, out MTRetCode requestResult)
        {
            List<CIMTUser> result = new List<CIMTUser>();
            requestResult = MTRetCode.MT_RET_ERROR;
            if (!CheckIfMannagerWorks()) return result;

            try
            {
                var myUserArray = m_manager.UserCreateArray();
                requestResult = m_manager.UserRequestArray(GroupName, myUserArray);

                if (requestResult == MTRetCode.MT_RET_OK)
                {
                    var realusers = myUserArray.ToArray();
                    foreach (var actUser in realusers)
                    {
                        result.Add(actUser);
                    }
                }
                else
                {
                    LogOut($"Error getting user: group_name={GroupName} error msg={requestResult}");
                }
            }
            catch (Exception ex)
            {
                LogOut($"Error getting user:  error={ex}");
            }

            return result;
        }


        //return list of all Deals at given UserLogin
        public List<CIMTDeal> GetAllDeals(ulong UserLogin, out MTRetCode requestResult, DateTime StartDate, DateTime EndDate)
        {
            List<CIMTDeal> result = new List<CIMTDeal>();
            requestResult = MTRetCode.MT_RET_ERROR;
            if (!CheckIfMannagerWorks()) return result;

            DateTime ZeroDate = new DateTime(1970, 1, 1);
            long startSecond = (long)(StartDate - ZeroDate).TotalSeconds;
            long stopSecond = (long)(EndDate - ZeroDate).TotalSeconds;

            try
            {
                var dealsList = m_manager.DealCreateArray();
                //requestResult = m_manager.DealRequest(UserLogin, 1, 3153600000, dealsList);
                requestResult = m_manager.DealRequest(UserLogin, startSecond, stopSecond, dealsList);

                if (requestResult == MTRetCode.MT_RET_OK)
                {
                    var realdeal = dealsList.ToArray();
                    foreach (var actDeal in realdeal)
                    {
                        result.Add(actDeal);
                    }
                }
                else
                {
                    LogOut($"Error getting Deals for user: user_Login={UserLogin} error msg={requestResult}");
                }
            }
            catch (Exception ex)
            {
                LogOut($"Error getting Deals for user: user_Login={UserLogin} error msg={ex}");
            }

            return result;
        }

        public void Dispose()
        {
            this.Shutdown();
        }


        //return user balance
        public double GetUserBalance(ulong UserLogin)
        {
            List<CIMTUser> result = new List<CIMTUser>();
            var requestResult = MTRetCode.MT_RET_ERROR;
            if (!CheckIfMannagerWorks()) return 0;

            try
            {
                var userAccount = m_manager.UserCreateAccount();
                requestResult = m_manager.UserAccountGet(UserLogin, userAccount);
                

                if (requestResult == MTRetCode.MT_RET_OK)
                {
                    return userAccount.Balance();
                }
                else
                {
                    LogOut($"Error getting user account: user_={UserLogin} error msg={requestResult}");
                }
            }
            catch (Exception ex)
            {
                LogOut($"Error getting user balance:  error={ex}");
            }

            return 0;
        }



        //subscribe some object to receive New Deals
        public void SubscribeDealSink(ref clsDealSink myDealSink)
        {
            if (!CheckIfMannagerWorks()) return;
            var resDealSink = myDealSink.RegisterSink();
            var requestResult1 = m_manager.DealSubscribe(myDealSink);
        }

        //subscribe some object to receive New connections
        public void SubscribeConnectSink(ref clsSinkManager mySManager)
        {
            if (!CheckIfMannagerWorks()) return;

            var resSManager = mySManager.RegisterSink();
            var subscribeResult = m_manager.Subscribe(mySManager);
        }


    }
}
