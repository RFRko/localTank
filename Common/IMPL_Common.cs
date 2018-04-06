using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanki;

namespace Common
{
    /// <summary>
	/// Абстрактный класс описующий информацию об объектах рендеринга.
	/// Является наследником IEntity.
	/// Используется в классах Tank, Bullet, Block.
	/// Наследующий класс обязан иметь атрибут [Serializable]
	/// </summary>
    [Serializable]
    public abstract class GameEntity : IEntity
    {
        private bool _can_shoot;
        private bool _can_destroy;
        private bool _is_alive;
        private int _speed;
        private Direction _direction;
        private Point _position;

        public GameEntity()
        {

        }

        public GameEntity(bool CanShoot, bool CanDestroy, bool IsAlive, int Speed, Point Position,Direction Direction)
        {
            this._can_destroy = CanDestroy;
            this._can_shoot = CanShoot;
            this._is_alive = IsAlive;
            this._speed = Speed;
            this._position = Position;
            this._direction = Direction;
        }
		public int Size { get; set; }

		/// <summary>
		/// Текущая позиция X Y.
		/// </summary>
		public Rectangle Position { get; set; }
		/// <summary>
		/// Направление движения.
		/// </summary>
		public Direction Direction
        {
            get { return this._direction; }
            set { this._direction = value; }
        }
        /// <summary>
        /// Возможно ли произвести выстрел в данный момент.
        /// </summary>
        public bool Can_Shoot
        {
            get { return this._can_shoot; }
            set { this._can_shoot = value; }
        }
        /// <summary>
        /// Состояние объект. 
        /// </summary>
        public bool Is_Alive
        {
            get { return this._is_alive; }
            set { this._is_alive = value; }
        }
        /// <summary>
        /// Может объект быть уничтожен.
        /// </summary>
        public bool Can_Be_Destroyed
        {
            get { return this._can_destroy; }
            set { this._can_destroy = value; }
        }
        /// <summary>
        /// Скорость движения объекта.
        /// </summary>
        public int Speed
        {
            get { return this._speed; }
            set { this._speed = value; }
        }

        public EntityAction Command { get; set; }
    }

    /// <summary>
	/// Класс описующий объект "Танк".
	/// Является наследником GameEntity.
    /// Создан для коммуникации между клиентом и сервером.
	/// </summary>
    /// 
    [Serializable]
    public class Tank : GameEntity, ITank
    {
        private int _lives;
        private Team _team;

        public Tank()
        {

        }

        public Tank(int Lives,Team Team,bool CanShoot, bool CanDestroy, bool IsAlive, int Speed, Point Position, Direction Direction) : base(CanShoot,CanDestroy,IsAlive,Speed,Position,Direction)
        {
            this._lives = Lives;
            this._team = Team;
        }

        public int Lives
        {
            get { return this._lives; }
            set { this._lives = value; }
        }

		public Guid Tank_ID { get; set; }

		public Team Team
        {
            get { return this._team; }
            set { this._team = value; }
        }
    }

    /// <summary>
	/// Класс описующий объект "Пуля".
	/// Является наследником GameEntity.
    /// Создан для коммуникации между клиентом и сервером.
	/// </summary>
    /// 
    [Serializable]
    public class Bullet : GameEntity, IBullet
    {
        private string _parent_id;

        public Bullet()
        {
            
        }

        public Bullet(string Parent_Id,bool CanShoot, bool CanDestroy, bool IsAlive, int Speed, Point Position, Direction Direction) : base(CanShoot,CanDestroy,IsAlive,Speed,Position,Direction)
        {
            this._parent_id = Parent_Id;
        }

        public Guid Parent_Id
        {
			get;
			set;
        }
    }

    /// <summary>
	/// Класс описующий объект "Блок"(пенек).
	/// Является наследником GameEntity.
    /// Создан для коммуникации между клиентом и сервером.
	/// </summary>
    /// 
    [Serializable]
    public class Block : GameEntity, IBlock
    {
        public Block()
        {

        }

        public Block(bool CanShoot, bool CanDestroy, bool IsAlive, int Speed, Point Position, Direction Direction) :base(CanShoot,CanDestroy,IsAlive,Speed,Position,Direction)
        {
                
        }
    }


    [Serializable]
    public class RoomStat : IRoomStat
    {
        public RoomStat() { }
        public int Players_count { get; set; }
		public Guid Pasport { get; set; }
		public Guid Creator_Pasport { get; set; }
	}


}
