using DeviceLibrary;

using System;
using System.Drawing;
using System.Windows.Forms;

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

        public Form2 form2;

        /// <summary>
        /// Instanciation de la classe principale de la dll
        /// </summary>
        private CDevicesManager deviceManage;

        /// <summary>
        /// Montant à payer
        /// </summary>
        private int ToPayByClient;

        /// <summary>
        /// Montant à distribuer par le BNR
        /// </summary>
        private int ToDispenseBNR;

        /// <summary>
        /// Indique si le montant perçu doit être remis à zéro.
        /// </summary>
        private bool isMontantPercuReset;

        /// <summary>
        /// Fonction executée à la fin de la saisie du montant à payer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TBAmountToPay_Leave(object sender, EventArgs e)
        {
            if(decimal.TryParse(tbToPay.Text, out decimal value))
            {
                ToPayByClient = (int)(value * 100);
                tbToPay.Text = $"{value:c2}";
                tbDenomination.Text = $"{0:c2}";
                deviceManage.BeginTransaction(ToPayByClient);
                if(isMontantPercuReset)
                {
                    tbReceived.Text = $"{ 0.00:c2}";
                }
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
        private void ButtonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Vide le champ de saisie.
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
            switch(e.reason)
            {
                case Reason.MONEYINTRODUCTED:
                {
                    a = () =>
                    {
                        CDevice.CInserted data = (CDevice.CInserted)((CDevice.CEvent)e.donnee).data;
                        int remaining = ToPayByClient - data.TotalAmount;
                        tbInfo.AppendText($"Espèces introduites : Canal {data.CVChannel} trieur {data.CVPath} valeur  {(decimal)data.ValeurCent / 100:c2}\r\n\r\n");
                        tbReceived.Text = $"{(decimal)data.TotalAmount / 100:c2}";
                        tbDenomination.Text = $"{(decimal)data.ValeurCent / 100:c2}";
                        if(remaining < 1)
                        {
                            tbRemaining.Text = $"{ 0.00:c2}";
                            if(data.TotalAmount > 0)
                            {
                                tbToPay.Text = $"{ 0.00:c2}";
                                isMontantPercuReset = true;
                            }
                        }
                        else
                        {
                            tbRemaining.Text = $"{(decimal)remaining / 100:c2}";
                        }
                        ButtonCounters_Click(sender, e);
                    };
                    break;
                }
                case Reason.COINVALIDATORERROR:
                {
                    a = () =>
                    {
                        CCoinValidator.CErroCV data = (CCoinValidator.CErroCV)((CDevice.CEvent)e.donnee).data;
                        tbInfo.AppendText($"Erreur coin validator - code : {data.code} raison : {data.errorText}\r\n\r\n");
                        ButtonCounters_Click(sender, e);
                    };
                    break;
                }
                case Reason.CASHCLOSED:
                {
                    a = () =>
                    {
                        stripLabelCashReaderStatus.BackColor = Color.Red;
                        stripLabelCashReaderStatus.Text = "Encaissement fermé";
                        ButtonCounters_Click(sender, e);
                    };
                    break;
                }
                case Reason.CASHOPENED:
                {
                    a = () =>
                    {
                        stripLabelCashReaderStatus.Text = "Encaissement Ouvert";
                        stripLabelCashReaderStatus.BackColor = Color.GreenYellow;
                        ButtonCounters_Click(sender, e);
                    };
                    break;
                }
                case Reason.HOPPERERROR:
                {
                    a = () =>
                    {
                        CHopper.CHopperError data = (CHopper.CHopperError)((CDevice.CEvent)e.donnee).data;
                        tbInfo.AppendText($"Erreur {data.Code} sur le {data.nameOfHopper}\r\n\r\n");
                        if(data.isHopperCritical)
                        {
                            MessageBox.Show(string.Format("Erreur {0} sur le {1}.\r\n", " Ce hopper est nécessaire au fonctionnement de la borne.", data.nameOfHopper));
                        }
                    };
                    break;
                }
                case Reason.HOPPERHWLEVELCHANGED:
                {
                    a = () =>
                    {
                        CHopper.CHardLevelData data = (CHopper.CHardLevelData)((CDevice.CEvent)e.donnee).data;
                        foreach(DataGridViewRow ligne in dataGridViewHopper.Rows)
                        {
                            if((bool)ligne.Cells["Present"].Value && (ligne.Cells["Identifiant"].Value.ToString() == data.nameOfHopper))
                            {
                                if((data.level == CDevice.CLevel.HardLevel.VIDE) || (data.level == CDevice.CLevel.HardLevel.PLEIN))
                                {
                                    ligne.Cells["LevelHW"].Style.BackColor = Color.Red;
                                    MessageBox.Show(string.Format("{0} critique", data.nameOfHopper));
                                }
                                else
                                {
                                    ligne.Cells["LevelHW"].Style.BackColor = Color.LimeGreen;
                                }
                                ligne.Cells["LevelHW"].Value = data.level;
                            }
                        }
                        ButtonCounters_Click(sender, e);
                    };
                    break;
                }
                case Reason.HOPPERSWLEVELCHANGED:
                {
                    a = () =>
                    {
                        CHopper.CSoftLevelData data = (CHopper.CSoftLevelData)((CDevice.CEvent)e.donnee).data;
                        foreach(DataGridViewRow ligne in dataGridViewHopper.Rows)
                        {
                            if((bool)ligne.Cells["Present"].Value && (ligne.Cells["Identifiant"].Value.ToString() == data.nameOfHopper))
                            {
                                if(data.level == CDevice.CLevel.SoftLevel.VIDE)
                                {
                                    MessageBox.Show(string.Format("{0} critique", data.nameOfHopper));
                                }
                                switch(data.level)
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
                                ligne.Cells["LevelSW"].Value = data.level;
                            }
                        }
                        ButtonCounters_Click(sender, e);
                    };
                    break;
                }
                case Reason.HOPPERDISPENSED:
                {
                    a = () =>
                    {
                        CHopper.CHopperStatus.CDispensedResult data = (CHopper.CHopperStatus.CDispensedResult)((CDevice.CEvent)e.donnee).data;
                        tbInfo.AppendText($"Distribution {data.nameOfHopper}\r\n");
                        tbInfo.AppendText($"Nombre de pièces à distribuer {data.CoinToDispense}\r\n");
                        tbInfo.AppendText($"Montant à distribuer {(decimal)data.AmountToDispense / 100:c2}\r\n");
                        tbInfo.AppendText($"Nombre de pièces distribuées : {data.CoinsPaid}\r\n");
                        tbInfo.AppendText($"Montant distribué : {(decimal)data.MontantPaid / 100:c2}\r\n");
                        tbInfo.AppendText($"Nombre de pièces non distribuées : {data.CoinsUnpaid}\r\n");
                        tbInfo.AppendText($"Montant non distribué : {(decimal)data.MontantUnpaid / 100:c2}\r\n\r\n");
                        ButtonCounters_Click(sender, e);
                    };
                    break;
                }
                case Reason.HOPPEREMPTIED:
                {
                    a = () =>
                    {
                        CHopper.CEmptyCount data = (CHopper.CEmptyCount)((CDevice.CEvent)e.donnee).data;
                        tbInfo.AppendText($"Vidage {data.nameOfHopper}\r\n");
                        tbInfo.AppendText($"Nombre de pièces {data.counter}\r\n");
                        tbInfo.AppendText($"Montant {(decimal)data.amountCounter / 100:c2}\r\n");
                        tbInfo.AppendText($"Différence en pièces {data.delta}\r\n");
                        tbInfo.AppendText($"Différence montant {(decimal)data.amountDelta / 100:c2}\r\n\r\n");
                        ButtonCounters_Click(sender, e);
                    };
                    break;
                }
                case Reason.DLLLREADY:
                {
                    a = () =>
                      {
                          byte byIndex = 0;
                          form2.Dispose();
                          if(deviceManage.Hoppers != null)
                          {
                              foreach(CHopper hopper in deviceManage.Hoppers)
                              {
                                  if(hopper.IsPresent)
                                  {
                                      dataGridViewHopper.Rows.Add(string.Format("{0}", hopper.name), (decimal)hopper.CoinValue / 100, hopper.IsPresent, 0, 0, "", "", false, hopper.DefaultFilling, false);
                                  }
                                  else
                                  {
                                      dataGridViewHopper.Rows.Add(string.Format("{0}", hopper.name), string.Empty, hopper.IsPresent);
                                  }
                                  dataGridViewHopper.Rows[byIndex].DefaultCellStyle.BackColor = hopper.IsPresent ? Color.LimeGreen : Color.Red;
                                  dataGridViewHopper["ToDispense", byIndex].Style.BackColor = Color.White;
                                  dataGridViewHopper["ToEmpty", byIndex].Style.BackColor =
                                  dataGridViewHopper["ToDispense", byIndex].Style.BackColor =
                                  dataGridViewHopper["ToLoad", byIndex].Style.BackColor =
                                  dataGridViewHopper["Reload", byIndex].Style.BackColor = Color.White;
                                  ++byIndex;
                              }
                          }
                          ButtonCounters_Click(sender, e);
                          stripLabelCom.Text = deviceManage.GetSerialPort();
                      };
                    break;
                }
                case Reason.BNRERREUR:
                {
                    a = () =>
                      {
                          MessageBox.Show(((CerrorBNR)((CDevice.CEvent)e.donnee).data).nameModule.ToString() + "\r\n" +
                          ((CerrorBNR)((CDevice.CEvent)e.donnee).data).error.ToString(), ((CDevice.CEvent)e.donnee).nameOfDevice.ToString());
                      };
                    break;
                }
                default:
                {
                    a = () =>
                    {
                    };
                    break;
                }
            }
            Invoke(a);
        }

        /// <summary>
        /// Quitte l'applciation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            deviceManage.Dispose();
            Application.Exit();
        }

        /// <summary>
        /// Lecture forcée des compteurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCounters_Click(object sender, EventArgs e)
        {
            dataGridViewCompteurs.Rows.Clear();
            dataGridViewCompteurs.Rows.Add("Total caisse", $"{(decimal)CccTalk.counters.totalAmountInCB / 100:c2}");
            dataGridViewCompteurs.Rows.Add("Total introduit", $"{(decimal)CccTalk.counters.totalAmountCashInCV / 100:c2}");
            dataGridViewCompteurs.Rows.Add("Total chargé", $"{(decimal)CccTalk.counters.totalAmountReload / 100:c2}");
            dataGridViewCompteurs.Rows.Add("Total rendu", $"{(decimal)CccTalk.counters.totalAmountCashOut / 100:c2}");
            dataGridViewCompteurs.Rows.Add("Total borne", $"{(decimal)CccTalk.counters.totalAmountInCabinet / 100:c2}");
            dataGridViewCompteurs.Rows.Add("Trop perçu", $"{(decimal)CccTalk.counters.amountOverPay / 100:c2}");
            if(deviceManage.monnayeur != null && deviceManage.monnayeur.canaux != null)
            {
                foreach(CCanal canal in deviceManage.monnayeur.canaux)
                {
                    dataGridViewCompteurs.Rows.Add($"CV nombre canal {canal.Number}", canal.CoinIn);
                    dataGridViewCompteurs.Rows.Add($"CV montant canal {canal.Number}", $"{(decimal)canal.MontantIn / 100:c2}");
                }
            }
            if(deviceManage.Hoppers != null)
            {
                foreach(CHopper hopper in deviceManage.Hoppers)
                {
                    dataGridViewCompteurs.Rows.Add($"{hopper.ToString()} niveau", hopper.CoinsInHopper);
                    dataGridViewCompteurs.Rows.Add($"{hopper.ToString()} montant in", $"{(decimal)hopper.AmountInHopper / 100:c2}");
                    dataGridViewCompteurs.Rows.Add($"{hopper.ToString()} nombre out", hopper.CoinsOut);
                    dataGridViewCompteurs.Rows.Add($"{hopper.ToString()} montant out", $"{(decimal)hopper.AmountOut / 100:c2}");
                    dataGridViewCompteurs.Rows.Add($"{hopper.ToString()} nombre rechargé", hopper.CoinsLoadedInHopper);
                    dataGridViewCompteurs.Rows.Add($"{hopper.ToString()} montant rechargé", $"{(decimal)hopper.AmountLoadedInHopper / 100:c2}");
                }
            }
        }

        /// <summary>
        /// Traite les flags dans les colonnes de la grille des hoppers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewHopper_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if((e.ColumnIndex == 7) && !(bool)dataGridViewHopper[e.ColumnIndex, e.RowIndex].Value)
                {
                    dataGridViewHopper["ToLoad", e.RowIndex].Value = false;
                    dataGridViewHopper["ToDispense", e.RowIndex].Value = 0;
                }
                if((e.ColumnIndex == 9) && !(bool)dataGridViewHopper[e.ColumnIndex, e.RowIndex].Value)
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
            foreach(CHopper hopper in deviceManage.Hoppers)
            {
                if(hopper.IsPresent)
                {
                    foreach(DataGridViewRow ligne in dataGridViewHopper.Rows)
                    {
                        if(hopper.name == ligne.Cells["Identifiant"].Value.ToString())
                        {
                            if(Convert.ToByte(ligne.Cells["ToDispense"].Value) > 0)
                            {
                                hopper.Dispense(Convert.ToByte(ligne.Cells["ToDispense"].Value));
                                ligne.Cells["ToDispense"].Value = 0.ToString();
                            }
                            else
                            {
                                if((bool)ligne.Cells["ToEmpty"].Value)
                                {
                                    ligne.Cells["ToEmpty"].Value = false;
                                    hopper.Empty();
                                }
                                else
                                {
                                    if((bool)ligne.Cells["ToLoad"].Value)
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
        /// Effectue le traitement des valeurs saisie dans la grille des hoppers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewHopper_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex > -1)
            {
                if(e.ColumnIndex == 3)
                {
                    if(byte.TryParse((string)dataGridViewHopper[e.ColumnIndex, e.RowIndex].Value, out byte number))
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
        /// Efface les logs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tbInfo.Clear();
        }

        /// <summary>
        /// Remise à zéro des compteurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRAZCompteurs_Click(object sender, EventArgs e)
        {
            deviceManage.ResetCounters();
            ButtonCounters_Click(sender, e);
        }

        /// <summary>
        /// Cloture de la transaction en cours.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonFinTrans_Click(object sender, EventArgs e)
        {
            deviceManage.EndTransaction();
        }

        private void BtnBNRDispense(object sender, EventArgs e)
        {
            deviceManage.BNRDispense(ToDispenseBNR);
        }

        private void TextBoxBNRDispense_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = "";
        }

        private void TextBoxBNRDspense_Leave(object sender, EventArgs e)
        {
            if(decimal.TryParse(textBoxBnrDispense.Text, out decimal value))
            {
                ToDispenseBNR = (int)(value * 100);
                textBoxBnrDispense.Text = $"{value:c2}";
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            form2 = new Form2();
            form2.Show();
            form2.Refresh();
            deviceManage = new CDevicesManager();
            deviceManage.CallAlert += new CDevicesManager.AlertEventHandler(MsgFromdll);
            isMontantPercuReset = false;
        }
    }
}