﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Audio;
using System.Xml;

namespace Kannon.Components
{
    class Sound : Component, IContentComponent
    {
        Dictionary<String, SoundEffect> m_Sounds;
        Dictionary<String, SoundEffectInstance> m_PlayingSounds;

        [ComponentCreator]
        public static Component Create(Entity ent, String name)
        {
            return new Sound(ent, name);
        }

        public Sound(Entity ent, String name) : base(ent, name)
        {
            m_Sounds = new Dictionary<string, SoundEffect>();
            m_PlayingSounds = new Dictionary<string, SoundEffectInstance>();
        }

        public void Load(Microsoft.Xna.Framework.Content.ContentManager cm)
        {
            for(int x = 0; x < m_Sounds.Count; x++)
            {
                String s = m_Sounds.ElementAt(x).Key;
                m_Sounds[s] = cm.Load<SoundEffect>(s);
            }
        }

        public void Play(String name)
        {
            if (m_PlayingSounds.ContainsKey(name))
                m_PlayingSounds[name].Resume();
            else if (m_Sounds.ContainsKey(name))
            {
                m_PlayingSounds.Add(name, m_Sounds[name].CreateInstance());
                m_PlayingSounds[name].Play();
            }
        }

        public void Pause(String name)
        {
            if (m_PlayingSounds.ContainsKey(name))
                m_PlayingSounds[name].Pause();
        }

        public override void Parse(System.Xml.XmlNode data)
        {
            foreach (XmlNode child in data)
            {
                if (child.Attributes["file"] != null)
                {
                    String filename = child.Attributes["file"].Value;
                    m_Sounds.Add(filename, null);
                    foreach (XmlNode innerChild in child)
                    {
                        switch (innerChild.Attributes["action"].Value.ToLower())
                        {
                            case "play":
                                Entity.AddEvent(innerChild.Attributes["event"].Value, (o) => Play(filename));
                                break;
                            case "pause":
                                Entity.AddEvent(innerChild.Attributes["event"].Value, (o) => Pause(filename));
                                break;
                        }
                    }
                }
            }
        }

    }
}