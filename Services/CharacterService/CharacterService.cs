using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
       
        private readonly IMapper _mapper;

        private readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            Character character = _mapper.Map<Character>(newCharacter);

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();                          //gen new id for char
            serviceResponse.Data = await _context.Characters
                    .Select(c => _mapper.Map<GetCharacterDto>(c))
                    .ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters(int userId)
        {
            var response = new ServiceResponse<List<GetCharacterDto>>();

            var dbCharacters = await _context.Characters
            .Where( c => c.User.Id == userId )
            .ToListAsync();

            response.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();

            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var DBcharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            serviceResponse.Data = _mapper.Map<GetCharacterDto>(DBcharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();

            try
            {


                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == updateCharacter.Id);
                _mapper.Map(updateCharacter, character);

                // character.Name = updateCharacter.Name;
                // character.Hitpoints = updateCharacter.Hitpoints;
                // character.Strength = updateCharacter.Strength;
                // character.Defense = updateCharacter.Defense;
                // character.Intelligence = updateCharacter.Intelligence;
                // character.Class = updateCharacter.Class;

                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetCharacterDto>(character);


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;

            }

            return response;



        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();

            try
            {

                var character = await _context.Characters.FirstAsync(c => c.Id == id);
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();

                response.Data = await  _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;

            }

            return response;



        }





    }
}