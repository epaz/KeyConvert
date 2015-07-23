//
// CommentsFrame.cs:
//
// Author:
//   Brian Nickel (brian.nickel@gmail.com)
//
// Original Source:
//   id3v2commentsframe.cpp from TagLib
//
// Copyright (C) 2005-2007 Brian Nickel
// Copyright (C) 2002,2003 Scott Wheeler (Original Implementation)
// 
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//
using System.Collections;
using System;

namespace TagLib.Id3v2
{
    /// <summary>
    ///    This class extends <see cref="Frame" />, implementing support for
    ///    ID3v2 InitialKey (TKEY) Frames.
    /// </summary>
    /// <remarks>
    ///    <para>A <see cref="InitialKeyFrame" /> should be used for storing
    ///    the musical key of the audio on the media file.</para>
    /// </remarks>
    public class InitialKeyFrame : Frame
    {
        #region Private Fields

        /// <summary>
        ///    Contains the text encoding to use when rendering the
        ///    current instance.
        /// </summary>
        private StringType encoding = Tag.DefaultEncoding;

        /// <summary>
        ///    Contains the ISO-639-2 language code of the current
        ///    instance.
        /// </summary>
        private string language = null;

        /// <summary>
        ///    Contains the description of the current instance.
        /// </summary>
        private string description = null;

        /// <summary>
        ///    Contains the key text of the current instance.
        /// </summary>
        private string text = null;

        #endregion



        #region Constructors

        /// <summary>
        ///    Constructs and initializes a new instance of <see
        ///    cref="InitialKeyFrame" /> with a specified description,
        ///    ISO-639-2 language code, and text encoding.
        /// </summary>
        /// <param name="description">
        ///    A <see cref="string" /> containing the description of the
        ///    new frame.
        /// </param>
        /// <param name="language">
        ///    A <see cref="string" /> containing the ISO-639-2 language
        ///    code of the new frame.
        /// </param>
        /// <param name="encoding">
        ///    A <see cref="StringType" /> containing the text encoding
        ///    to use when rendering the new frame.
        /// </param>
        /// <remarks>
        ///    When a frame is created, it is not automatically added to
        ///    the tag. Consider using <see cref="Get" /> for more
        ///    integrated frame creation.
        /// </remarks>
        public InitialKeyFrame(string description, string language,
                              StringType encoding)
            : base(FrameType.TKEY, 4)
        {
            this.encoding = encoding;
            this.language = language;
            this.description = description;
        }

        /// <summary>
        ///    Constructs and initializes a new instance of <see
        ///    cref="InitialKeyFrame" /> with a specified description and
        ///    ISO-639-2 language code.
        /// </summary>
        /// <param name="description">
        ///    A <see cref="string" /> containing the description of the
        ///    new frame.
        /// </param>
        /// <param name="language">
        ///    A <see cref="string" /> containing the ISO-639-2 language
        ///    code of the new frame.
        /// </param>
        /// <remarks>
        ///    When a frame is created, it is not automatically added to
        ///    the tag. Consider using <see cref="Get" /> for more
        ///    integrated frame creation.
        /// </remarks>
        public InitialKeyFrame(string description, string language)
            : this(description, language,
                    TagLib.Id3v2.Tag.DefaultEncoding)
        {
        }

        /// <summary>
        ///    Constructs and initializes a new instance of <see
        ///    cref="InitialKeyFrame" /> with a specified description.
        /// </summary>
        /// <param name="description">
        ///    A <see cref="string" /> containing the description of the
        ///    new frame.
        /// </param>
        /// <remarks>
        ///    When a frame is created, it is not automatically added to
        ///    the tag. Consider using <see cref="Get" /> for more
        ///    integrated frame creation.
        /// </remarks>
        public InitialKeyFrame(string description)
            : this(description, null)
        {
        }

        /// <summary>
        ///    Constructs and initializes a new instance of <see
        ///    cref="InitialKeyFrame" /> by reading its raw data in a
        ///    specified ID3v2 version.
        /// </summary>
        /// <param name="data">
        ///    A <see cref="ByteVector" /> object starting with the raw
        ///    representation of the new frame.
        /// </param>
        /// <param name="version">
        ///    A <see cref="byte" /> indicating the ID3v2 version the
        ///    raw frame is encoded in.
        /// </param>
        public InitialKeyFrame(ByteVector data, byte version)
            : base(data, version)
        {
            SetData(data, 0, version, true);
        }

        /// <summary>
        ///    Constructs and initializes a new instance of <see
        ///    cref="InitialKeyFrame" /> by reading its raw data in a
        ///    specified ID3v2 version.
        /// </summary>
        /// <param name="data">
        ///    A <see cref="ByteVector" /> object containing the raw
        ///    representation of the new frame.
        /// </param>
        /// <param name="offset">
        ///    A <see cref="int" /> indicating at what offset in
        ///    <paramref name="data" /> the frame actually begins.
        /// </param>
        /// <param name="header">
        ///    A <see cref="FrameHeader" /> containing the header of the
        ///    frame found at <paramref name="offset" /> in the data.
        /// </param>
        /// <param name="version">
        ///    A <see cref="byte" /> indicating the ID3v2 version the
        ///    raw frame is encoded in.
        /// </param>
        protected internal InitialKeyFrame(ByteVector data, int offset,
                                          FrameHeader header,
                                          byte version)
            : base(header)
        {
            SetData(data, offset, version, false);
        }

        #endregion



        #region Public Properties

        /// <summary>
        ///    Gets and sets the text encoding to use when storing the
        ///    current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the text encoding to
        ///    use when storing the current instance.
        /// </value>
        /// <remarks>
        ///    This encoding is overridden when rendering if <see
        ///    cref="Tag.ForceDefaultEncoding" /> is <see
        ///    langword="true" /> or the render version does not support
        ///    it.
        /// </remarks>
        public StringType TextEncoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        /// <summary>
        ///    Gets and sets the ISO-639-2 language code stored in the
        ///    current instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the ISO-639-2 language
        ///    code stored in the current instance.
        /// </value>
        /// <remarks>
        ///    There should only be one file with a matching description
        ///    and ISO-639-2 language code per tag.
        /// </remarks>
        public string Language
        {
            get
            {
                if (language != null && language.Length > 2)
                    return language.Substring(0, 3);

                return "XXX";
            }
            set { language = value; }
        }

        /// <summary>
        ///    Gets and sets the description stored in the current
        ///    instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the description
        ///    stored in the current instance.
        /// </value>
        /// <remarks>
        ///    There should only be one frame with a matching
        ///    description and ISO-639-2 language code per tag.
        /// </remarks>
        public string Description
        {
            get
            {
                if (description != null)
                    return description;

                return string.Empty;
            }
            set { description = value; }
        }

        /// <summary>
        ///    Gets and sets the initial key text stored in the current
        ///    instance.
        /// </summary>
        /// <value>
        ///    A <see cref="string" /> containing the initial key text
        ///    stored in the current instance.
        /// </value>
        public string Text
        {
            get
            {
                if (text != null)
                    return text;

                return string.Empty;
            }
            set { text = value; }
        }

        #endregion



        #region Public Methods

        /// <summary>
        ///    Gets a string representation of the current instance.
        /// </summary>
        /// <returns>
        ///    A <see cref="string" /> containing the initial key text.
        /// </returns>
        public override string ToString()
        {
            return Text;
        }

        #endregion



        #region Public Static Methods

        /// <summary>
        ///    Gets a specified initial key frame from the specified tag,
        ///    optionally creating it if it does not exist.
        /// </summary>
        /// <param name="tag">
        ///    A <see cref="Tag" /> object to search in.
        /// </param>
        /// <param name="description">
        ///    A <see cref="string" /> specifying the description to
        ///    match.
        /// </param>
        /// <param name="language">
        ///    A <see cref="string" /> specifying the ISO-639-2 language
        ///   code to match.
        /// </param>
        /// <param name="create">
        ///    A <see cref="bool" /> specifying whether or not to create
        ///    and add a new frame to the tag if a match is not found.
        /// </param>
        /// <returns>
        ///    A <see cref="InitialKeyFrame" /> object containing the
        ///    matching frame, or <see langword="null" /> if a match
        ///    wasn't found and <paramref name="create" /> is <see
        ///    langword="false" />.
        /// </returns>
        public static InitialKeyFrame Get(Tag tag, string description,
                                         string language, bool create)
        {
            InitialKeyFrame key;
            foreach (Frame frame in tag.GetFrames(FrameType.TKEY))
            {
                key = frame as InitialKeyFrame;

                if (key == null)
                    continue;

                if (key.Description != description)
                    continue;

                if (language != null && language != key.Language)
                    continue;

                return key;
            }

            if (!create)
                return null;

            key = new InitialKeyFrame(description, language);
            tag.AddFrame(key);
            return key;
        }

        /// <summary>
        ///    Gets a specified initial key frame from the specified
        ///    tag, trying to to match the language but accepting one
        ///    with a different language if a match was not found.
        /// </summary>
        /// <param name="tag">
        ///    A <see cref="Tag" /> object to search in.
        /// </param>
        /// <param name="description">
        ///    A <see cref="string" /> specifying the description to
        ///    match.
        /// </param>
        /// <param name="language">
        ///    A <see cref="string" /> specifying the ISO-639-2 language
        ///   code to match.
        /// </param>
        /// <returns>
        ///    A <see cref="InitialKeyFrame" /> object containing the
        ///    matching frame, or <see langword="null" /> if a match
        ///    wasn't found.
        /// </returns>
        /// <remarks>
        ///    <para>The method tries matching with the following order
        ///    of precidence:</para>
        ///    <list type="number">
        ///       <item><term>The first frame with a matching
        ///       description and language.</term></item>
        ///       <item><term>The first frame with a matching
        ///       language.</term></item>
        ///       <item><term>The first frame with a matching
        ///       description.</term></item>
        ///       <item><term>The first frame.</term></item>
        ///    </list>
        /// </remarks>
        public static InitialKeyFrame GetPreferred(Tag tag,
                                                  string description,
                                                  string language)
        {
            InitialKeyFrame best = null;
            foreach (Frame f in tag.GetFrames(FrameType.TKEY))
            {
                var cf = f as InitialKeyFrame;
                if (cf == null)
                    continue;

                if (cf.Language == language)
                    return cf;

                if (best == null)
                    best = cf;
            }

            return best;
        }

        #endregion



        #region Protected Methods

        /// <summary>
        ///    Populates the values in the current instance by parsing
        ///    its field data in a specified version.
        /// </summary>
        /// <param name="data">
        ///    A <see cref="ByteVector" /> object containing the
        ///    extracted field data.
        /// </param>
        /// <param name="version">
        ///    A <see cref="byte" /> indicating the ID3v2 version the
        ///    field data is encoded in.
        /// </param>
        protected override void ParseFields(ByteVector data,
                                             byte version)
        {
            if (data.Count < 4)
                throw new CorruptFileException(
                    "Not enough bytes in field.");

            encoding = (StringType)data[0];
            language = data.ToString(StringType.Latin1, 1, 3);

            // Instead of splitting into two string, in the format
            // [{desc}\0{value}], try splitting into three strings
            // in case of a misformatted [{desc}\0{value}\0].
            string[] split = data.ToStrings(encoding, 4, 3);

            if (split.Length == 0)
            {
                // No data in the frame.
                description = String.Empty;
                text = String.Empty;
            }
            else if (split.Length == 1)
            {
                // Bad comment frame. Assume that it lacks a
                // description.
                description = String.Empty;
                text = split[0];
            }
            else
            {
                description = split[0];
                text = split[1];
            }
        }

        /// <summary>
        ///    Renders the values in the current instance into field
        ///    data for a specified version.
        /// </summary>
        /// <param name="version">
        ///    A <see cref="byte" /> indicating the ID3v2 version the
        ///    field data is to be encoded in.
        /// </param>
        /// <returns>
        ///    A <see cref="ByteVector" /> object containing the
        ///    rendered field data.
        /// </returns>
        protected override ByteVector RenderFields(byte version)
        {
            StringType encoding = CorrectEncoding(TextEncoding,
                version);
            ByteVector v = new ByteVector();

            v.Add((byte)encoding);
            v.Add(ByteVector.FromString(Language, StringType.Latin1));
            v.Add(ByteVector.FromString(description, encoding));
            v.Add(ByteVector.TextDelimiter(encoding));
            v.Add(ByteVector.FromString(text, encoding));

            return v;
        }

        #endregion



        #region ICloneable

        /// <summary>
        ///    Creates a deep copy of the current instance.
        /// </summary>
        /// <returns>
        ///    A new <see cref="Frame" /> object identical to the
        ///    current instance.
        /// </returns>
        public override Frame Clone()
        {
            var frame = new InitialKeyFrame(description,
                language, encoding);
            frame.text = text;
            return frame;
        }

        #endregion
    }
}
