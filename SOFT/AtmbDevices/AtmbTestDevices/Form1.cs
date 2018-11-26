﻿using System;
using System.Drawing;
using System.Windows.Forms;
using DeviceLibrary;
using System.Threading;

namespace AtmbTestDevices
{
    /// <summary>
    /// Fenêtre principale du program de test de la dll DeviceLibrary
    /// </summary>
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Instanciation de la classe principale de la dll
        /// </summary>
        private CDevicesManage deviceManage;

        /// <summary>
        /// Montant à payer 
        /// </summary>
        private int ToPayByClient;

        /// <summary>
        /// Fonction exectutée lors du chargement de la fenêtre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Création des objets</remarks>
        private void Form1_Load(object sender, EventArgs e)
        {
            deviceManage = new CDevicesManage();
            deviceManage.CallAlert += new CDevicesManage.AlertEventHandler(MsgFromdll);
        }

        /// <summary>
        /// Fonction executée à la fin de la saisie du montant à payer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TBAmountToPay_Leave(object sender, EventArgs e)
        {
            if (decimal.TryParse(tbToPay.Text, out decimal value))
            {
                ToPayByClient = (int)(value * 100);
                tbToPay.Text = $"{value:c2}";
                deviceManage.SetToPay(ToPayByClient);
            }
            else
            {
                ((TextBox)sender).Text = "0,00 €";
            }
        }

        /// <summary>
        /// Fonction executée lors du click sur le bouton quitter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TBAmountToPay_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = "";
        }

        /// <summary>
        /// Fonction levée à chaque évenement provenant de la dll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgFromdll(object sender, CalertEventArgs e)
        {
            Action a;
            switch (e.reason)
            {
                case Reason.MONEYINTRODUCTED:
                {
                    a = () =>
                    {
                        int remaining = (ToPayByClient - CDevice.denominationInserted.TotalAmount);
                        tbInfo.AppendText($"Pièce reconnue : Canal {((CDevice.CInserted)e.donnee).CVChannel} trieur {((CDevice.CInserted)e.donnee).CVPath} valeur  {(decimal)((CDevice.CInserted)e.donnee).ValeurCent / 100:c2}\r\n\r\n");
                        tbReceived.Text = $"{((decimal)((CDevice.CInserted)e.donnee).TotalAmount) / 100:c2}";
                        if (remaining < 1)
                        {
                            tbRemaining.Text = $"{ (0.00):c2}";
                        }
                        else
                        {
                            tbRemaining.Text = $"{((decimal)remaining) / 100:c2}";
                            tbDenomination.Text = $"{((decimal)((CDevice.CInserted)e.donnee).ValeurCent) / 100:c2}";
                        }

                    };
                    break;
                }
                case Reason.COINVALIDATORERROR:
                {
                    a = () =>
                    {
                        tbInfo.AppendText($"Erreur coin validator - code : { ((CCoinValidator.CErroCV)e.donnee).code} raison : { ((CCoinValidator.CErroCV)e.donnee).errorText}\r\n\r\n");
                    };
                    break;
                }
                case Reason.CASHCLOSED:
                {
                    a = () =>
                    {
                        stripLabelCashReaderStatus.BackColor = Color.Red;
                        stripLabelCashReaderStatus.Text = "Encaissement fermé";
                    };
                    break;
                }
                case Reason.CASHOPENED:
                {
                    stripLabelCashReaderStatus.BackColor = Color.GreenYellow;
                    a = () => stripLabelCashReaderStatus.Text = "Encaissement Ouvert";
                    break;
                }
                case Reason.HOPPERERROR:
                {
                    a = () => { };
                    break;
                }
                case Reason.HOPPERHWLEVELCHANGED:
                {
                    a = () =>
                    {
                        foreach (DataGridViewRow ligne in dataGridViewHopper.Rows)
                        {
                            if ((bool)(ligne.Cells["Present"].Value) && (ligne.Cells["Identifiant"].Value.ToString() == ((CDevice.CLevel)e.donnee).ID))
                            {
                                ligne.Cells["LevelHW"].Value = ((CDevice.CLevel)e.donnee).hardLevel;
                                if ((((CDevice.CLevel)e.donnee).hardLevel == CDevice.CLevel.HardLevel.VIDE) || (((CDevice.CLevel)e.donnee).hardLevel == CDevice.CLevel.HardLevel.PLEIN))
                                {
                                    ligne.Cells["LevelHW"].Style.BackColor = Color.Red;
                                }
                                else
                                {
                                    ligne.Cells["LevelHW"].Style.BackColor = Color.LimeGreen;
                                }
                                break;
                            }
                        }
                    };
                    break;
                }

                case Reason.HOPPERSWLEVELCHANGED:
                {
                    a = () =>
                    {
                        foreach (DataGridViewRow ligne in dataGridViewHopper.Rows)
                        {
                            if ((bool)ligne.Cells["Present"].Value && (ligne.Cells["Identifiant"].Value.ToString() == ((CDevice.CLevel)e.donnee).ID))
                            {
                                ligne.Cells["LevelSW"].Value = ((CDevice.CLevel)e.donnee).softLevel;
                                switch (((CDevice.CLevel)e.donnee).softLevel)
                                {
                                    case CDevice.CLevel.SoftLevel.PLEIN:
                                    case CDevice.CLevel.SoftLevel.VIDE:
                                    {
                                        ligne.Cells["LevelSW"].Style.BackColor = Color.Red;
                                        break;
                                    }
                                    case CDevice.CLevel.SoftLevel.HAUT:
                                    case CDevice.CLevel.SoftLevel.BAS:
                                    {
                                        ligne.Cells["LevelSW"].Style.BackColor = Color.Orange;
                                        break;
                                    }
                                    default:
                                    {
                                        ligne.Cells["LevelSW"].Style.BackColor = Color.LimeGreen;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    };
                    break;
                }
                case Reason.HOPPERDISPENSED:
                {
                    a = () =>
                    {
                        tbInfo.AppendText($"Distribution hopper {((CHopper.CHopperStatus.CDispensedResult)e.donnee).HopperNumber}\r\n");
                        tbInfo.AppendText($"Nombre de pièces à distribuer {((CHopper.CHopperStatus.CDispensedResult)e.donnee).CoinToDispense}\r\n");
                        tbInfo.AppendText($"Montant à distribuer {(decimal)((CHopper.CHopperStatus.CDispensedResult)e.donnee).AmountToDispense / 100:c2}\r\n");
                        tbInfo.AppendText($"Nombre de pièces distribuées : {((CHopper.CHopperStatus.CDispensedResult)e.donnee).CoinsPaid}\r\n");
                        tbInfo.AppendText($"Montant distribué : {(decimal)((CHopper.CHopperStatus.CDispensedResult)e.donnee).MontantPaid / 100:c2}\r\n");
                        tbInfo.AppendText($"Nombre de pièces non distribuées : {((CHopper.CHopperStatus.CDispensedResult)e.donnee).CoinsUnpaid}\r\n");
                        tbInfo.AppendText($"Montant non distribué : {(decimal)((CHopper.CHopperStatus.CDispensedResult)e.donnee).MontantUnpaid / 100:c2}\r\n\r\n");
                    };
                    break;
                }
                case Reason.HOPPEREMPTIED:
                {
                    a = () =>
                    {
                        tbInfo.AppendText($"Vidage hopper {((CHopper)e.donnee).Number}\r\n");
                        tbInfo.AppendText($"Nombre de pièces {((CHopper)e.donnee).emptyCount.counter}\r\n");
                        tbInfo.AppendText($"Montant {(decimal)((CHopper)e.donnee).emptyCount.amountCounter / 100:c2}\r\n");
                        tbInfo.AppendText($"Différence en pièces {((CHopper)e.donnee).emptyCount.delta}\r\n");
                        tbInfo.AppendText($"Différence montant {(decimal)((CHopper)e.donnee).emptyCount.amountDelta / 100:c2}\r\n\r\n");
                    };
                    break;
                }
                case Reason.DLLLREADY:
                {
                    a=()=>
                    {
                        byte byIndex = 0;
                        foreach (CHopper hopper in deviceManage.Hoppers)
                        {
                            if (hopper.IsPresent)
                            {
                                dataGridViewHopper.Rows.Add(string.Format("Hopper {0}", hopper.Number), (decimal)hopper.CoinValue / 100, hopper.IsPresent, 0, 0, "", "", false, hopper.DefaultFilling, false);
                            }
                            else
                            {
                                dataGridViewHopper.Rows.Add(string.Format("Hopper {0}", hopper.Number), string.Empty, hopper.IsPresent);
                            }
                            dataGridViewHopper.Rows[byIndex].DefaultCellStyle.BackColor = hopper.IsPresent ? Color.LimeGreen : Color.Red;
                            dataGridViewHopper["ToDispense", byIndex].Style.BackColor = Color.White;
                            dataGridViewHopper["ToEmpty", byIndex].Style.BackColor =
                            dataGridViewHopper["ToDispense", byIndex].Style.BackColor =
                            dataGridViewHopper["ToLoad", byIndex].Style.BackColor =
                            dataGridViewHopper["Reload", byIndex].Style.BackColor = Color.White;

                            ++byIndex;
                        }
                        ButtonCounters_Click(sender, e);
                        stripLabelCom.Text = deviceManage.GetSerialPort();
                        Refresh();
                    };
                    break;
                }
                default:
                    a = () => { };
                    break;
            }
            Invoke(a);            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        ///// <param name="e"></param>
        private void FormMain_Shown(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            deviceManage.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCounters_Click(object sender, EventArgs e)
        {
            dataGridViewCompteurs.Rows.Clear();
            dataGridViewCompteurs.Rows.Add("Total introduit", string.Format("{0:c2}", (decimal)CccTalk.counters.totalAmountCashIn / 100));
            dataGridViewCompteurs.Rows.Add("Total rendu", string.Format("{0:c2}", (decimal)CccTalk.counters.totalAmountCashOut / 100));
            dataGridViewCompteurs.Rows.Add("Total borne", string.Format("{0:c2}", (decimal)CccTalk.counters.totalAmountCash / 100));
            dataGridViewCompteurs.Rows.Add("Trop perçu", string.Format("{0:c2}", (decimal)CccTalk.counters.amountOverPay / 100));
            foreach (CCanal canal in deviceManage.monnayeur.canaux)
            {
                dataGridViewCompteurs.Rows.Add($"CV nombre canal {canal.Number}", canal.CoinIn);
                dataGridViewCompteurs.Rows.Add($"CV montant canal {canal.Number}", $"{(decimal)canal.MontantIn / 100:c2}");
            }
            foreach (CHopper hopper in deviceManage.Hoppers)
            {
                dataGridViewCompteurs.Rows.Add($"{hopper.ToString()} niveau", hopper.CoinsInHopper);
                dataGridViewCompteurs.Rows.Add($"{hopper.ToString()} montant in", $"{(decimal)hopper.AmountInHopper / 100:c2}");
                dataGridViewCompteurs.Rows.Add($"{hopper.ToString()} nombre out", hopper.CoinsOut);
                dataGridViewCompteurs.Rows.Add($"{hopper.ToString()} montant out", $"{(decimal)hopper.AmountOut / 100:c2}");
                dataGridViewCompteurs.Rows.Add($"{hopper.ToString()} nombre rechargé", hopper.CoinsLoadedInHopper);
                dataGridViewCompteurs.Rows.Add($"{hopper.ToString()} montant rechargé", $"{(decimal)hopper.AmountLoadedInHopper / 100:c2}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewHopper_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((e.ColumnIndex == 7) && !(bool)dataGridViewHopper[e.ColumnIndex, e.RowIndex].Value)
                {
                    dataGridViewHopper["ToLoad", e.RowIndex].Value = false;
                    dataGridViewHopper["ToDispense", e.RowIndex].Value = 0;
                }
                if ((e.ColumnIndex == 9) && !(bool)dataGridViewHopper[e.ColumnIndex, e.RowIndex].Value)
                {
                    dataGridViewHopper["ToEmpty", e.RowIndex].Value = false;
                }
                dataGridViewHopper.CurrentCell.Value = !(bool)dataGridViewHopper.CurrentCell.Value;
            }
            catch
            {

            }
        }

        /// <summary>
        /// Traitement des hoppers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonHoppers_Click(object sender, EventArgs e)
        {
            foreach (CHopper hopper in deviceManage.Hoppers)
            {
                if (hopper.IsPresent)
                {
                    foreach (DataGridViewRow ligne in dataGridViewHopper.Rows)
                    {
                        if (hopper.ToString() == ligne.Cells["Identifiant"].Value.ToString())
                        {
                            if (Convert.ToByte(ligne.Cells["ToDispense"].Value) > 0)
                            {
                                hopper.Distribute(Convert.ToByte(ligne.Cells["ToDispense"].Value));
                                ligne.Cells["ToDispense"].Value = 0.ToString();
                            }
                            else
                            {
                                if ((bool)ligne.Cells["ToEmpty"].Value)
                                {
                                    ligne.Cells["ToEmpty"].Value = false;
                                    hopper.Empty();
                                }
                                else
                                {
                                    if ((bool)ligne.Cells["ToLoad"].Value)
                                    {
                                        ligne.Cells["ToLoad"].Value = false;
                                        hopper.LoadHopper(Convert.ToInt64(ligne.Cells["Reload"].Value));
                                        ligne.Cells["Reload"].Value = string.Format("{0}", hopper.DefaultFilling);
                                    }
                                }
                            }
                            ButtonCounters_Click(sender, e);
                            Refresh();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewHopper_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if (e.ColumnIndex == 3)
                {
                    if (byte.TryParse((string)dataGridViewHopper[e.ColumnIndex, e.RowIndex].Value, out byte number))
                    {
                        dataGridViewHopper["ToLoad", e.RowIndex].Value = false;
                        dataGridViewHopper["ToEmpty", e.RowIndex].Value = false;
                    }
                    else
                    {
                        MessageBox.Show("Le nombre de pièces doit être compris entre 1 et 255");
                        dataGridViewHopper[e.ColumnIndex, e.RowIndex].Value = 0.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tbInfo.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRAZCompteurs_Click(object sender, EventArgs e)
        {
           CccTalk.ResetCounters();
            ButtonCounters_Click(sender, e);
        }
    }
}