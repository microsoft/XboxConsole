using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xbox.XTF;
using Microsoft.Xbox.XTF.Input;
using Microsoft.Xbox.Input;

namespace Microsoft.Xbox.Input
{

    public sealed partial class XtfGamepad
    {
        private GAMEPAD_REPORT _currentState;
        private bool _connected;
        private ulong _id;
        private InputClient _client;
        
        private const ushort _allButtons = 0xffff;
        private const float _triggerConversion = 0.003922f;
        
        
        public XtfGamepad(string address)        
        {
            this._client = new InputClient(address);
            this._connected = false;            
        }
                
        public void Connect()
        {
            if(!_connected)
            {
                _id = _client.ConnectGamepad();
                if(_id >= 0)            
                {
                    _connected = true;
                }
                else                
                {
                    throw new XtfInputException("The controller could not be connected");
                }
            }
            else            
            {
                throw new InvalidOperationException("Already connected");
            }
        }
        
        public void Disconnect()        
        {
            if(_connected)
            {
                _client.DisconnectGamepad(_id);
                _connected = false;
            }
            else            
            {
                throw new InvalidOperationException("Not currently connected");
            }
        }
       
        public void PressButtons(GamepadButtons buttons)
        {
            _currentState.Buttons |= (ushort)buttons;
            SendReport();
        }
        
        public void UnpressButtons(GamepadButtons buttons)
        {            
            ushort inverseButtons = (ushort)(_allButtons ^ (ushort)buttons); //xor
            
            _currentState.Buttons &= (ushort)inverseButtons;
            SendReport();
        }
        
        public void PullLeftTrigger(float value)
        {
            _currentState.LeftTrigger = (ushort)((short)(value / _triggerConversion) * 4);
            SendReport();
        }
        
        public void PullRightTrigger(float value)
        {
            _currentState.RightTrigger = (ushort)((short)(value / _triggerConversion) * 4);
            SendReport();
        }
        
        public void MoveLeftThumbstick(float X, float Y)        
        {
            if((!(-1 <= X) && !(X >= 1)) && (!(-1 <= Y) && !(Y >= 1)))
            {
                throw new ArgumentException("Values out of range");
            }
            
            _currentState.LeftThumbstickX = (short)(X * short.MaxValue);
            _currentState.LeftThumbstickY = (short)(Y * short.MaxValue);
            SendReport();
        }
        
        public void MoveRightThumbstick(float X, float Y)
        {
            if((!(-1 <= X) && !(X >= 1)) && (!(-1 <= Y) && !(Y >= 1)))
            {
                throw new ArgumentException("Values out of range");
            }
            
            _currentState.RightThumbstickX = (short)(X * short.MaxValue);
            _currentState.RightThumbstickY = (short)(Y * short.MaxValue);
            SendReport();
        }
        
        public GAMEPAD_REPORT CurrentState
        {
            get{ return _currentState; }
        }
        
        private void SendReport()
        {
            if(_connected)
            {
                _client.SendGamepadReport(_id, _currentState);
            }
        }
        
    }
}    