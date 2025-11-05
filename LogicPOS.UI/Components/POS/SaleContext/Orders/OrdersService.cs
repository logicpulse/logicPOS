using LogicPOS.Api.Features.Orders.AddTicket;
using LogicPOS.Api.Features.Orders.CloseOrder;
using LogicPOS.Api.Features.Orders.CreateOrder;
using LogicPOS.Api.Features.Orders.GetAllOrders;
using LogicPOS.Api.Features.Orders.ReduceItems;
using LogicPOS.Api.Features.Orders.RemoveTicketItem;
using LogicPOS.Api.Features.Orders.SplitTicket;
using LogicPOS.Api.Features.POS.Orders.Orders.Common;
using LogicPOS.Api.Features.POS.Orders.Orders.DeleteOrder;
using LogicPOS.Api.Features.POS.Orders.Orders.GetOrderById;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Services
{
    public static class OrdersService
    {
        private static List<Order> GetOpenOrders(Guid? tableId = null)
        {
            var getResult = DependencyInjection.Mediator.Send(new GetOpenOrdersQuery(tableId)).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult);
                return null;
            }

            return getResult.Value.ToList();
        }


        public static IEnumerable<PosOrder> GetOpenPosOrders(Guid? tableId = null)
        {
            var orders = GetOpenOrders(tableId);

            if (orders == null)
            {
                return null;
            }

            var posOrders = new List<PosOrder>();

            foreach (var order in orders)
            {
                var posOrder = new PosOrder(order);
                posOrders.Add(posOrder);
            }

            return posOrders;
        }

        public static bool SaveOrder(PosOrder posOrder)
        {
            var command = new CreateOrderCommand();
            command.TableId = posOrder.Table.Id;
            command.Tickets = posOrder.Tickets.Select(t => new CreateTicketDto
            {
                Details = t.Items.Select(i => new CreateOrderDetailDto
                {
                    ArticleId = i.Article.Id,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                })
            });

            var createResult = DependencyInjection.Mediator.Send(command).Result;

            if (createResult.IsError)
            {
                ErrorHandlingService.HandleApiError(createResult);
                return false;
            }

            posOrder.Id = createResult.Value;

            return true;
        }

        public static bool SavePosTicket(PosOrder order, PosTicket ticket)
        {
            var command = new AddTicketCommand();
            command.OrderId = order.Id.Value;
            command.Details = ticket.Items.Select(i => new CreateOrderDetailDto
            {
                ArticleId = i.Article.Id,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            });

            var createResult = DependencyInjection.Mediator.Send(command).Result;

            if (createResult.IsError)
            {
                ErrorHandlingService.HandleApiError(createResult);
                return false;
            }

            return true;
        }

        public static bool CloseOrder(Guid orderId)
        {
            var closeResult = DependencyInjection.Mediator.Send(new CloseOrderCommand { OrderId = orderId }).Result;

            if (closeResult.IsError)
            {
                ErrorHandlingService.HandleApiError(closeResult);
                return false;
            }

            return true;
        }

        public static bool ReduceOrderItems(Guid orderId, IEnumerable<SaleItem> items)
        {
            var command = new ReduceOrderItemsCommand();
            items = SaleItem.Compact(items);

            command.OrderId = orderId;
            command.Items = items.Select(i => new ReduceOrderItemDto
            {
                ArticleId = i.Article.Id,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();

            var reduceResult = DependencyInjection.Mediator.Send(command).Result;

            if (reduceResult.IsError)
            {
                ErrorHandlingService.HandleApiError(reduceResult);
                return false;
            }

            return true;
        }

        public static bool MoveTicketItem(Guid orderId, Guid tableId,SaleItem item)
        {
            var command = new MoveTicketItemCommand();
           
            var removeItem= new ReduceOrderItemDto
            {
                ArticleId = item.Article.Id,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };
            command.TableId= tableId;
            command.OrderId = orderId;
            command.Item = removeItem;

            var removeResult = DependencyInjection.Mediator.Send(command).Result;

            if (removeResult.IsError)
            {
                ErrorHandlingService.HandleApiError(removeResult);
                return false;
            }

            return true;
        }


        public static bool SplitTicket(Guid orderId, IEnumerable<SaleItem> items, int splittersNumber)
        {
            var command = new SplitTicketCommand();
            items = SaleItem.Compact(items);

            command.OrderId = orderId;
            command.SplittersNumber=splittersNumber;
            command.Items = items.Select(i => new SplitTicketDto
            {
                ArticleId = i.Article.Id,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();

            var splitTicketResult = DependencyInjection.Mediator.Send(command).Result;

            if (splitTicketResult.IsError)
            {
                ErrorHandlingService.HandleApiError(splitTicketResult);
                return false;
            }

            return true;
        }

        public static PosOrder GetPosOrder(Guid orderId)
        {
            var getResult = DependencyInjection.Mediator.Send(new GetOrderByIdQuery(orderId)).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult);
                return null;
            }

            return new PosOrder(getResult.Value);
        }

        public static bool DeleteOrder(Guid orderId, string reason)
        {
            var closeResult = DependencyInjection.Mediator.Send(new DeleteOrderCommand(orderId,reason)).Result;

            if (closeResult.IsError)
            {
                ErrorHandlingService.HandleApiError(closeResult);
                return false;
            }

            return true;
        }

        public static bool DeleteOrder(Guid orderId)
        {
            var closeResult = DependencyInjection.Mediator.Send(new DeleteOrderCommand(orderId, "?")).Result;

            if (closeResult.IsError)
            {
                ErrorHandlingService.HandleApiError(closeResult);
                return false;
            }

            return true;
        }
    }
}
