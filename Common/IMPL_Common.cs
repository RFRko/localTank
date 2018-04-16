using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tanki;

namespace Tanki
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
        private bool _can_destroy;
        private bool _is_alive;
		private int _size;
        private Direction _direction;
        private Rectangle _position;
        private EntityAction _command;

        public GameEntity()
        {

        }

        public GameEntity(bool CanDestroy, bool IsAlive, Rectangle Position,Direction Direction,int size,EntityAction Command)
        {
            this._command = Command;
            this._can_destroy = CanDestroy;
            this._is_alive = IsAlive;
            this._position = Position;
            this._direction = Direction;
			this._size = size;
        }


		/// <summary>
		/// Текущая позиция X Y.
		/// </summary>
		public Rectangle Position
        {
            get { return this._position; }
            set { this._position = value; }
        }
		/// <summary>
		/// Направление движения.
		/// </summary>
		public Direction Direction

        {
            get { return this._direction; }
            set { this._direction = value; }
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

        public EntityAction Command
        {
            get { return this._command; }
            set { this._command = value; }
        }

		public int Size
		{
			get { return this._size; }
			set { this._size = value; }
		}
		//public override bool Equals(object obj)
		//{
		//	return base.Equals(obj);
		//}
		//public override int GetHashCode()
		//{
		//	return base.GetHashCode();
		//}
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
		    private Guid _tank_ID;
        private bool _can_shoot;
        private int _speed;
        public Tank()
        {

        }

        public Tank(int Lives,Team Team,bool CanShoot, bool CanDestroy, bool IsAlive, int Speed, Rectangle Position, Direction Direction,int Size, Guid TankID, EntityAction Command) : base(CanDestroy,IsAlive,Position,Direction,Size,Command)
        {
            this._lives = Lives;
            this._team = Team;
            this._speed = Speed;
            this._can_shoot = CanShoot;
        }

        public int Lives
        {
            get { return this._lives; }
            set { this._lives = value; }
        }
		public string Name { get; set; }

		    public Guid Tank_ID
        {
            get { return this._tank_ID; }
            set { this._tank_ID = value; }
        }

		    public Team Team
        {
            get { return this._team; }
            set { this._team = value; }
        }

        public int Speed
        {
            get { return this._speed; }
            set { this._speed = value; }
        }

        public bool Can_Shoot
        {
            get { return this._can_shoot; }
            set { this._can_shoot= value; }
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
        private Guid _parent_id;
        private int _speed;

        public Bullet()
        {
            
        }

        public Bullet(Guid Parent_Id,bool CanShoot, bool CanDestroy, bool IsAlive, int Speed, Rectangle Position, Direction Direction,int Size,EntityAction Command) : base(CanDestroy,IsAlive,Position,Direction,Size,Command)
        {
            this._parent_id = Parent_Id;
        }

        public Guid Parent_Id
        {
            get { return this._parent_id; }
            set { this._parent_id = value; }
        }

        public int Speed
        {
            get { return this._speed; }
            set { this._speed = value; }
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
		public BlockType blockType { get; set; }
		public Block()
        {

        }

        public Block(bool CanDestroy, bool IsAlive, Rectangle Position, Direction Direction,int Size, EntityAction Command) :base(CanDestroy,IsAlive,Position,Direction,Size,Command)
        {
                
        }
    }


    // [Serializable]
    // public class RoomStat : IRoomStat
    // {
    //     public RoomStat() { }
    //     public int Players_count { get; set; }
    //   public Guid Pasport { get; set; }
    //   public Guid Creator_Pasport { get; set; }
    //}

    //[Serializable]
    //public class Addresssee : IAddresssee
    //{
//
//        public RoomStat() { }
//        public int Players_count { get; set; }
//		    public Guid Pasport { get; set; }
//		    public Guid Creator_Pasport { get; set; }
//	  }

    [Serializable]
    public class Addresssee : IAddresssee
    {
        public Addresssee(IPEndPoint ep) { RemoteEndPoint = ep; }
        public IPEndPoint RemoteEndPoint { get;}
	}


    public class GameObjectFactory : IGameObjectFactory
    {
        public IBlock CreateBlock()
        {
            return new Block();
        }

        public IBullet CreateBullet()
        {
            return new Bullet();
        }


        public ITank CreateTank()
        {
            return new Tank();
        }
    }
	[Serializable]
	public class Map : IMap
	{
		public IEnumerable<IBlock> Blocks { get; set; }
		public IEnumerable<IBullet> Bullets { get; set; }
		public IEnumerable<ITank> Tanks { get; set; }
	}
}
