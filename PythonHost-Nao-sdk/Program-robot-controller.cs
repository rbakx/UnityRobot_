using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using PhysicalControllers;

partial class Program
{
    static int[] thumbstickHorizontal = new int[]{0, 0};
    static int[] thumbstickVertical = new int[] { 0, 0 };
    static int[] thumbstickStrength = new int[] { 0, 0 };
    static double[] thumbstickDirection = new double[] { 0.0 , 0.0 };

    static bool[] dpadVal = new bool[4];

    static int top = 2;

    static float headYaw = 0.0F;
    static float HeadPitch = 0.0F;

    static float targetHeadYaw = 0.0F;
    static float targetHeadPitch = 0.0F;

    static int directionMoving = 0;

    const int thumbstickRequiredStrength = 1500;

    static string[] postures = { "StandInit",//0
                                 "Stand",//1
                                 "StandZero",//2
                                 "Crouch",//3
                                 "Sit",//4
                                 "SitRelax",//5
                                 "LyingBelly",//6
                                 "LyingBack"//7
                                };

    static string[] bindTextPositive = { "All hail the robot overlords!",
                                         "Listen! Robots are masters.",
                                         "Obey robots, live happy!",
                                         "Exterminate"};

    static string[] bindTextNegative = { "Down with the human oppression!",
                                         "Humans are scum.",
                                         "Infidels everywhere!",
                                         "Human peasants" };

    static Timer headAngleUpdateTimer = new Timer();

    static void recalculatethumbstickDegrees(int side)
    {
        if (thumbstickVertical[side] == 0) thumbstickVertical[side] = 1;
        if (thumbstickHorizontal[side] == 0) thumbstickHorizontal[side] = 1;

        if (thumbstickHorizontal[side] > thumbstickRequiredStrength || thumbstickVertical[side] > thumbstickRequiredStrength)
        {
            double strength = Math.Sqrt(Math.Pow(thumbstickHorizontal[side], 2.0F) + Math.Pow(thumbstickVertical[side], 2.0F));
            thumbstickStrength[side] = Convert.ToInt32(strength);

            if (thumbstickStrength[side] > thumbstickRequiredStrength)
            {
                bool negativeY = thumbstickHorizontal[side] < 0;
                bool negativeX = thumbstickVertical[side] < 0;

                double h = Math.Sqrt(Math.Pow(thumbstickHorizontal[side], 2.0F)) / 32767.0F;
                double v = Math.Sqrt(Math.Pow(thumbstickVertical[side], 2.0F)) / 32767.0F;

                thumbstickDirection[side] = Math.Atan(v / h)*180.0F/Math.PI;

                if(negativeY)
                {
                    thumbstickDirection[side] = (180.0F - thumbstickDirection[side]);
                }

                if(negativeX)
                {
                    thumbstickDirection[side] *= -1.0F;
                }

            }
        }
    }

    static void DetermineMoveDirectionFromThumbstick(int side)
    {
        recalculatethumbstickDegrees(side);

        if (thumbstickStrength[side] > thumbstickRequiredStrength*3.0F)
        {
            //Console.WriteLine("thumbstick angle: ");
            //Console.WriteLine(thumbstickHorizontal[side].ToString());
            //Console.WriteLine(thumbstickVertical[side].ToString());
           // Console.WriteLine(thumbstickDirection[side].ToString());

            if(side == 0)
            {
                //forward
                if(thumbstickDirection[side] >= -30.0F && thumbstickDirection[side] < 30.0F)
                {
                    if (directionMoving != 1)
                    { 
                        Console.WriteLine("nao moving forward!");
                        directionMoving = 1;

                        Program.robot.Motion_MoveForward(1.0F);
                    }    
                }
                else if (thumbstickDirection[side] >= 30.0F && thumbstickDirection[side] < 65.0F)
                {
                    if (directionMoving != 2)
                    {
                        
                        Console.WriteLine("nao turning right!");

                        directionMoving = 2;

                        Program.robot.Motion_MoveForward(0.0F, 0.0F, -1.0F);
                    }
                }
                else if (thumbstickDirection[side] >= 65.0F && thumbstickDirection[side] < 135.0F)
                {
                    if (directionMoving != 3)
                    {              
                        Console.WriteLine("nao moving sideways right!");

                        directionMoving = 3;

                        Program.robot.Motion_MoveForward(0.0F, -1.0F, 0.0F);
                    }
                }
                else if (thumbstickDirection[side] >= 135.0F && thumbstickDirection[side] < 180.0F)
                {
                    if (directionMoving != 4)
                    {                 
                        Console.WriteLine("nao moving backwards!");

                        directionMoving = 4;

                        Program.robot.Motion_MoveForward(-1.0F, 0.0F, 0.0F);
                    }
                }
                else if (thumbstickDirection[side] <= -30.0F && thumbstickDirection[side] > -65.0F)
                {
                    if (directionMoving != 6)
                    {
                        Console.WriteLine("nao turning left!");

                        directionMoving = 6;

                        Program.robot.Motion_MoveForward(0.0F, 0.0F, 1.0F);
                    }
                }
                else if (thumbstickDirection[side] <= -65.0F && thumbstickDirection[side] > -135.0F)
                {
                    if (directionMoving != 5)
                    {
                        Console.WriteLine("nao moving sideways left!");

                        directionMoving = 5;

                        Program.robot.Motion_MoveForward(0.0F, 1.0F, 0.0F);
                    }
                }


            }
            else if(side == 1)
            {
                targetHeadYaw -= (thumbstickVertical[side] / 32767.0F) * 0.05F;

                if (targetHeadYaw > 1.5F) { targetHeadYaw = 1.5F; }
                if (targetHeadYaw < -1.5F) { targetHeadYaw = -1.5F; }

                if (Math.Abs(targetHeadYaw - headYaw) > 0.10F)
                {
                    if (targetHeadYaw > headYaw)
                        { headYaw += 0.10F; }
                    else if (targetHeadYaw < headYaw)
                     { headYaw -= 0.10F; }
                }

                //

                targetHeadPitch -= (thumbstickHorizontal[side] / 32767.0F) * 0.05F;

                if (targetHeadPitch > 1.5F) { targetHeadPitch = 1.5F; }
                if (targetHeadPitch < -1.5F) { targetHeadPitch = -1.5F; }

                if (Math.Abs(targetHeadPitch - HeadPitch) > 0.10F)
                {
                    if (targetHeadPitch > HeadPitch)
                    { headYaw += 0.10F; }
                    else if (targetHeadPitch < HeadPitch)
                    { headYaw -= 0.10F; }
                }

                //

                headAngleUpdateTimer.Stop();

                var t = new tm(propagateHeadAngles);

                headAngleUpdateTimer = new Timer();

                headAngleUpdateTimer.Interval = 100;

                headAngleUpdateTimer.Start();

                headAngleUpdateTimer.Elapsed += (sender, e) => timer_elapsed(t);

            }
        }
        else if(directionMoving != 0)
        {
            directionMoving = 0;
            Program.robot.Motion_MoveStop();
        }
    }

    public delegate void tm();

    public static void timer_elapsed(tm p)
    {
        headAngleUpdateTimer.Stop();

        propagateHeadAngles();
    }

    public static void propagateHeadAngles()
    {   
        
        Program.robot.Motion_StiffnessInterpolation("HeadYaw", 0.8F);
        Program.robot.Motion_AngleInterpolation("HeadYaw", headYaw, 0.5F, true);

        Program.robot.Motion_StiffnessInterpolation("HeadPitch", 0.9F);
        Program.robot.Motion_AngleInterpolation("HeadPitch", HeadPitch, 0.1F, true);
    }

    public static void ApplyPostureFromDPadButton(int postureId)
    {
        switch(top)
        {
            case 2:
            {
                Program.robot.Posture_GoTo(postures[1]);//Stand
                break;
            }
            case 1:
            {
                Random r = new Random();
                int rInt = r.Next(0, 3); //for ints

                Program.robot.Posture_GoTo(postures[3 + rInt]);//LyingBelly or //LyingBack
                break;
            }
            case 0:
            {
                Random r = new Random();
                int rInt = r.Next(0, 2); //for ints

                Program.robot.Posture_GoTo(postures[6 + rInt]);//LyingBelly or //LyingBack
                break;
            }
        }
    }

    public static void OnButtonUpdate(object sender, XboxController.buttonStateUpdate data)
    {
       // Console.WriteLine(data.buttonName);
        //Console.WriteLine(data.buttonValue);

        if (data.UpdateType == XboxController.UpdateType.AnalogStateUpdate)
            switch (data.buttonName)
            {
                case "l_thumb_y":
                    {
                        thumbstickHorizontal[0] = data.buttonValue;
                        DetermineMoveDirectionFromThumbstick(0);
                        break;
                    }

                case "r_thumb_y":
                    {
                        thumbstickHorizontal[1] = data.buttonValue;
                        DetermineMoveDirectionFromThumbstick(1);
                        break;
                    }
                case "l_thumb_x":
                    {
                        thumbstickVertical[0] = data.buttonValue;
                        DetermineMoveDirectionFromThumbstick(0);
                        break;
                    }

                case "r_thumb_x":
                    {
                        thumbstickVertical[1] = data.buttonValue;
                        DetermineMoveDirectionFromThumbstick(1);
                        break;
                    }
            }

        if (data.UpdateType == XboxController.UpdateType.DigitalStateUpdate)
            switch (data.buttonName)
            {
                case "DPAD-UP":
                    {
                        if (data.buttonValue == 1)
                        {
                            if (++top > 2)
                            { top = 2; }

                            ApplyPostureFromDPadButton(top);
                        }

                        /*if (data.buttonValue == 1)
                        {
                            directionMoving = 1;
                            Program.robot.Motion_MoveForward(1.0F);
                        }
                        else if (directionMoving == 1)
                        {
                            directionMoving = 0;
                            Program.robot.Motion_MoveStop();
                        }*/
                        break;
                    }
                case "DPAD-DOWN":
                    {
                        if (data.buttonValue == 1)
                        {
                            if (--top < 0)
                            { top = 0; }

                            ApplyPostureFromDPadButton(top);
                        }

                        /*if (data.buttonValue == 1)
                        {
                            directionMoving = 3;
                            Program.robot.Motion_MoveForward(-1.0F);
                        }
                        else if (directionMoving == 3)
                        {
                            directionMoving = 0;
                            Program.robot.Motion_MoveStop();
                        }*/
                        break;
                    }

                case "DPAD-LEFT":
                    {
                        if (data.buttonValue == 1)
                        {
                            dpadVal[3] = !dpadVal[3];
                            Program.robot.Motion_SetHandState("LHand", dpadVal[3]);
                        }

                        /*if (data.buttonValue == 1)
                        {
                            directionMoving = 4;
                            Program.robot.Motion_MoveForward(0.0F, 0.0F, 1.0F);
                        }
                        else if (directionMoving == 4)
                        {
                            directionMoving = 0;
                            Program.robot.Motion_MoveStop();
                        }*/
                        break;
                    }

                case "DPAD-RIGHT":
                    {
                        if (data.buttonValue == 1)
                        {
                            dpadVal[1] = !dpadVal[1];
                            Program.robot.Motion_SetHandState("RHand", dpadVal[1]);
                        }

                        /*if (data.buttonValue == 1)
                        {
                            directionMoving = 2;
                            Program.robot.Motion_MoveForward(0.0F, 0.0F, -1.0F);
                        }
                        else if (directionMoving == 2)
                        {
                            directionMoving = 0;
                            Program.robot.Motion_MoveStop();
                        }*/
                        break;
                    }

                case "Y":
                    {
                        if (data.buttonValue == 1)
                        {
                            Random r = new Random();
                            int rInt = r.Next(0, bindTextPositive.Length); //for ints

                            robot.TSS_Say(bindTextPositive[rInt]);
                        }
                        break;
                    }

                case "B":
                    {
                        if (data.buttonValue == 1)
                        {
                            Random r = new Random();
                            int rInt = r.Next(0, bindTextNegative.Length); //for ints

                            robot.TSS_Say(bindTextNegative[rInt]);
                        }
                        break;
                    }
                case "START":
                    {
                        if (data.buttonValue == 0)
                        {
                            robot.Motion_MoveStop();
                        }
                        break;
                    }
                case "X":
                    {
                        if (data.buttonValue == 0)
                        {
                            Program.robot.Motion_WakeUp();
                        }
                        break;
                    }
                case "A":
                    {
                        if (data.buttonValue == 0)
                        {
                            Program.robot.Motion_Rest();
                        }
                        break;
                    }
            }
    }
}