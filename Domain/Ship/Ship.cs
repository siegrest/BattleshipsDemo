using System;
using System.Collections.Generic;

namespace Domain.Ship {
    public class Ship {
        public readonly string Title;
        public readonly char Symbol;
        public readonly int Size;
        public Pos ShipPos;
        public ShipDirection Direction;
        private ShipStatus[] _shipStatuses;

        public Ship(string shipTitle, char shipSymbol, int shipSize) {
            Symbol = shipSymbol;
            Title = shipTitle;
            Size = shipSize;

            CreateShipStatusBlocks();
        }
        
        private Ship(Ship ship, int size) {
            Symbol = ship.Symbol;
            Title = ship.Title;
            Size = size;

            CreateShipStatusBlocks();
        }
        
        public void SetLocation(Pos pos, ShipDirection direction) {
            Direction = direction;
            ShipPos = new Pos(pos);
        }

        public bool CheckIfIntersect(Pos newPos, int newSize, ShipDirection newDirection, int padding) {
            // Ship hasn't been placed yet
            if (ShipPos == null) {
                return false;
            }

            // This is one retarded way of doing it, but hey, it works...
            for (int offset = 0; offset < newSize; offset++) {
                var newPosOffsetX = newPos.X + (newDirection == ShipDirection.Right ? offset : 0);
                var newPosOffsetY = newPos.Y + (newDirection == ShipDirection.Right ? 0 : offset);

                for (int i = 0; i < Size; i++) {
                    var shipPosX = ShipPos.X + (Direction == ShipDirection.Right ? i : 0);
                    var shipPosY = ShipPos.Y + (Direction == ShipDirection.Right ? 0 : i);
                    
                    if (shipPosX >= newPosOffsetX - padding && shipPosX <= newPosOffsetX + padding && 
                        shipPosY >= newPosOffsetY - padding && shipPosY <= newPosOffsetY + padding) {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsAtPos(Pos pos) {
            // Ship hasn't been placed yet
            if (ShipPos == null) {
                return false;
            }

            for (int i = 0; i < Size; i++) {
                var shipBlockPosX = ShipPos.X + (Direction == ShipDirection.Right ? i : 0);
                var shipBlockPosY = ShipPos.Y + (Direction == ShipDirection.Right ? 0 : i);

                if (shipBlockPosX == pos.X && shipBlockPosY == pos.Y) {
                    return true;
                }
            }

            return false;
        }

        public AttackResult AttackAtPos(Pos pos) {
            if (!IsAtPos(pos)) {
                // Should be checked beforehand
                throw new ArgumentException(nameof(pos));
            }
            
            // Depending on the direction of the ship, set the local status tracker to hit
            if (Direction == ShipDirection.Right) {
                _shipStatuses[pos.X - ShipPos.X] = ShipStatus.Hit;
            } else {
                _shipStatuses[pos.Y - ShipPos.Y] = ShipStatus.Hit;
            }

            return IsDestroyed() ? AttackResult.Sink : AttackResult.Hit;
        }

        public bool IsDestroyed() {
            foreach (var status in _shipStatuses) {
                if (status == ShipStatus.Ok) {
                    return false;
                }
            }

            return true;
        }

        private void CreateShipStatusBlocks() {
            _shipStatuses = new ShipStatus[Size];

            for (int i = 0; i < Size; i++) {
                _shipStatuses[i] = ShipStatus.Ok;
            }
        }

        
        
        
        
        private static readonly Ship Carrier, Battleship, Submarine, Cruiser, Patrol;
        
        static Ship() {
            Carrier = new Ship("Carrier", 'C', 5);
            Battleship = new Ship("Battleship", 'B', 4);
            Submarine = new Ship("Submarine", 'S', 3);
            Cruiser = new Ship("Cruiser", 'c', 2);
            Patrol = new Ship("Patrol", 'p', 1);
        }

        public static List<Ship> GenShipSet(List<Rule> rules) {
            var ships = new List<Ship>();
            
            // Create the number of ships defined by users
            for (int i = 0; i < Rule.GetRule(rules, Rule.CarrierCount); i++) {
                ships.Add(new Ship(Carrier, Rule.CarrierSize.Value));
            }
            for (int i = 0; i < Rule.GetRule(rules, Rule.BattleshipCount); i++) {
                ships.Add(new Ship(Battleship, Rule.BattleshipSize.Value));
            }
            for (int i = 0; i < Rule.GetRule(rules, Rule.SubmarineCount); i++) {
                ships.Add(new Ship(Submarine, Rule.SubmarineSize.Value));
            }
            for (int i = 0; i < Rule.GetRule(rules, Rule.CruiserCount); i++) {
                ships.Add(new Ship(Cruiser, Rule.CruiserSize.Value));
            }
            for (int i = 0; i < Rule.GetRule(rules, Rule.PatrolCount); i++) {
                ships.Add(new Ship(Patrol, Rule.PatrolSize.Value));
            }

            return ships;
        }
    }
}