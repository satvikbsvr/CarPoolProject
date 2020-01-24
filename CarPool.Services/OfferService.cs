﻿using System;
using System.Collections.Generic;
using System.Text;
using CarPool.Model;
using CarPool.Database;
using CarPool.Enums;

namespace CarPool.Services
{
    public class OfferService
    {
        public Offer CreateOffer(string driverName, string userId, Location fromLocation, Location toLocation, int availability, string vehicleRegNumber, string vehicleModel)
        {
            Offer NewOffer = new Offer(driverName, userId, fromLocation, toLocation, availability, vehicleRegNumber, vehicleModel, IEnums.OfferStatus.Active);
            DataBase.Offers.Add(NewOffer);
            return NewOffer;
        }

        public void EndOffer(string riderId)
        {
            foreach (var offer in DataBase.Offers)
            {
                if (offer.RiderId.Equals(riderId) && offer.Status != IEnums.OfferStatus.Ended)
                {
                    UpdateOfferStatus(offer, IEnums.OfferStatus.Ended);
                    PaymentService NewPaymentDue = new PaymentService();
                    NewPaymentDue.AddPaymentDue(riderId);
                    BookingService BookingService = new BookingService();
                    BookingService.EndAllRides(riderId);
                }
            }
        }

        public bool VehicleVerification(string vehicleNumber)
        {
            foreach (var offer in DataBase.Offers)
            {
                if (offer.VehicleRegNumber.Equals(vehicleNumber))
                {
                    return false;
                }
            }
            return true;
        }

        public List<Offer> DisplayActiveOffers(string fromLocation, string toLocation, int numberOfPassengers)
        {
            List<Offer> ActiveOffers = new List<Offer>();
            foreach (var offer in DataBase.Offers)
            {
                if (offer.FromLocation.Equals(fromLocation) && offer.ToLocation.Equals(toLocation) && offer.Status.Equals(IEnums.OfferStatus.Active) && offer.Availability >= numberOfPassengers)
                {
                    ActiveOffers.Add(offer);
                }
            }
            return ActiveOffers;
        }

        public bool CheckAvailability(string riderId, int numberOfPassengers)
        {
            foreach (var offer in DataBase.Offers)
            {
                if (offer.RiderId.Equals(riderId) && offer.Status.Equals(IEnums.OfferStatus.Active))
                {
                    if (numberOfPassengers < offer.Availability)
                        return true;
                }
            }
            return false;
        }

        public void UpdateAvailability(string riderId, int numberOfPassengers)
        {
            foreach (var offer in DataBase.Offers)
            {
                if (offer.RiderId.Equals(riderId) && offer.Status.Equals(IEnums.OfferStatus.Active))
                {
                    offer.Availability -= numberOfPassengers;
                    if (offer.Availability == 0)
                        UpdateOfferStatus(offer, IEnums.OfferStatus.OutOfSeats);
                }
            }
        }

        public void AddViaPoint(Offer offer, IEnums.LocationIndex locationIndex)
        {
            foreach(var location in DataBase.Locations)
            {
                if(location.Index.Equals(locationIndex))
                {
                    offer.ViaPoints.Add(location);
                    break;
                }
            }
        }

        public void UpdateOfferStatus(Offer offer, IEnums.OfferStatus status)
        {
            offer.Status = status;
        }

        public bool AnyActiveOffer(string userId)
        {
            foreach(var offer in DataBase.Offers)
            {
                if (offer.RiderId.Equals(userId) && offer.Status != IEnums.OfferStatus.Ended)
                    return true;
            }
            return false;
        }

        public Offer GetDriverDetails(string riderId)
        {
            foreach (var offer in DataBase.Offers)
            {
                if (offer.RiderId.Equals(riderId) && offer.Status != IEnums.OfferStatus.Ended)
                    return offer;
            }
            return null;
        }

        public List<Offer> DisplayOffersHistory(string userId)
        {
            List<Offer> AllOffers = new List<Offer>();
            foreach (var offer in DataBase.Offers)
            {
                if (offer.RiderId.Equals(userId))
                    AllOffers.Add(offer);
            }
            return AllOffers;
        }

        public void CancelOffer(string userId)
        {
            foreach(var offer in DataBase.Offers)
            {
                if (offer.RiderId.Equals(userId) && (offer.Status.Equals(IEnums.OfferStatus.Active) || offer.Status.Equals(IEnums.OfferStatus.OutOfSeats)) )
                {
                    offer.Status = IEnums.OfferStatus.Ended;
                    break;
                }
            }
        }
    }
}